using System;
using System.Collections.Generic;
using System.Text;

namespace KCP.Common 
{
    /// <summary>
    /// Provides a resource pool that enables reusing instances of type <see cref="T:T[]"/>. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// Renting and returning buffers with an <see cref="ArrayPool{T}"/> can increase performance
    /// in situations where arrays are created and destroyed frequently, resulting in significant
    /// memory pressure on the garbage collector.
    /// </para>
    /// <para>
    /// This class is thread-safe.  All members may be used by multiple threads concurrently.
    /// </para>
    /// </remarks>
    public abstract class ArrayPool<T> 
    {
        /// <summary>
        /// Creates a new <see cref="ArrayPool{T}"/> instance using default configuration options.
        /// </summary>
        /// <returns>A new <see cref="ArrayPool{T}"/> instance.</returns>
        public static ArrayPool<T> Create()
        {
            return new DefaultArrayPool<T>();
        }

        /// <summary>
        /// Creates a new <see cref="ArrayPool{T}"/> instance using custom configuration options.
        /// </summary>
        /// <param name="maxArrayLength">The maximum length of array instances that may be stored in the pool.</param>
        /// <param name="maxArraysPerBucket">
        /// The maximum number of array instances that may be stored in each bucket in the pool.  The pool
        /// groups arrays of similar lengths into buckets for faster access.
        /// </param>
        /// <returns>A new <see cref="ArrayPool{T}"/> instance with the specified configuration options.</returns>
        /// <remarks>
        /// The created pool will group arrays into buckets, with no more than <paramref name="maxArraysPerBucket"/>
        /// in each bucket and with those arrays not exceeding <paramref name="maxArrayLength"/> in length.
        /// </remarks>
        public static ArrayPool<T> Create(int maxArrayLength, int maxArraysPerBucket) 
        {
            return new DefaultArrayPool<T>(maxArrayLength, maxArraysPerBucket);
        }
        /// <summary>
        /// .net系统自己管理
        /// </summary>
        /// <returns></returns>
        public static ArrayPool<T> System() 
        {
            return new SysArrayPool<T>();
        }
        /// <summary>
        /// Retrieves a buffer that is at least the requested length.
        /// </summary>
        /// <param name="minimumLength">The minimum length of the array needed.</param>
        /// <returns>
        /// An <see cref="T:T[]"/> that is at least <paramref name="minimumLength"/> in length.
        /// </returns>
        /// <remarks>
        /// This buffer is loaned to the caller and should be returned to the same pool via 
        /// <see cref="Return"/> so that it may be reused in subsequent usage of <see cref="Rent"/>.  
        /// It is not a fatal error to not return a rented buffer, but failure to do so may lead to 
        /// decreased application performance, as the pool may need to create a new buffer to replace
        /// the one lost.
        /// </remarks>
        public abstract T[] Rent(int minimumLength);

        /// <summary>
        /// Returns to the pool an array that was previously obtained via <see cref="Rent"/> on the same 
        /// <see cref="ArrayPool{T}"/> instance.
        /// </summary>
        /// <param name="array">
        /// The buffer previously obtained from <see cref="Rent"/> to return to the pool.
        /// </param>
        /// <param name="clearArray">
        /// If <c>true</c> and if the pool will store the buffer to enable subsequent reuse, <see cref="Return"/>
        /// will clear <paramref name="array"/> of its contents so that a subsequent consumer via <see cref="Rent"/> 
        /// will not see the previous consumer's content.  If <c>false</c> or if the pool will release the buffer,
        /// the array's contents are left unchanged.
        /// </param>
        /// <remarks>
        /// Once a buffer has been returned to the pool, the caller gives up all ownership of the buffer 
        /// and must not use it. The reference returned from a given call to <see cref="Rent"/> must only be
        /// returned via <see cref="Return"/> once.  The default <see cref="ArrayPool{T}"/>
        /// may hold onto the returned buffer in order to rent it again, or it may release the returned buffer
        /// if it's determined that the pool already has enough buffers stored.
        /// </remarks>
        public abstract void Return(T[] array, bool clearArray = false);
    }

    public sealed class SysArrayPool<T> : ArrayPool<T> 
    {
        public override T[] Rent(int minimumLength) 
        {
            return new T[minimumLength];
        }

        public override void Return(T[] array, bool clearArray = false)
        {

        }
    }

    public sealed partial class DefaultArrayPool<T> : ArrayPool<T> 
    {
        /// <summary>The default maximum length of each array in the pool (2^20).</summary>
        private const int DefaultMaxArrayLength = 1024 * 1024;
        /// <summary>The default maximum number of arrays per bucket that are available for rent.</summary>
        private const int DefaultMaxNumberOfArraysPerBucket = 50;
        /// <summary>Lazily-allocated empty array used when arrays of length 0 are requested.</summary>
        private static T[] s_emptyArray; // we support contracts earlier than those with Array.Empty<T>()

        private readonly Bucket[] _buckets;

        public DefaultArrayPool() : this(DefaultMaxArrayLength, DefaultMaxNumberOfArraysPerBucket) 
        {
            Id = GetHashCode();
        }

        public DefaultArrayPool(int maxArrayLength, int maxArraysPerBucket)
        {
            Id = GetHashCode();
            if(maxArrayLength <= 0) 
            {
                throw new ArgumentOutOfRangeException("maxArrayLength");
            }
            if(maxArraysPerBucket <= 0) 
            {
                throw new ArgumentOutOfRangeException("maxArraysPerBucket");
            }

            // Our bucketing algorithm has a min length of 2^4 and a max length of 2^30.
            // Constrain the actual max used to those values.
            const int MinimumArrayLength = 0x10, MaximumArrayLength = 0x40000000;
            if(maxArrayLength > MaximumArrayLength) 
            {
                maxArrayLength = MaximumArrayLength;
            }
            else if(maxArrayLength < MinimumArrayLength) 
            {
                maxArrayLength = MinimumArrayLength;
            }

            // Create the buckets.
            int poolId = Id;
            int maxBuckets = Utilities.SelectBucketIndex(maxArrayLength);
            var buckets = new Bucket[maxBuckets + 1];
            for(int i = 0; i < buckets.Length; i++) 
            {
                buckets[i] = new Bucket(Utilities.GetMaxSizeForBucket(i), maxArraysPerBucket, poolId);
            }
            _buckets = buckets;
        }

        /// <summary>Gets an ID for the pool to use with events.</summary>
        private int Id = 0;

        public override T[] Rent(int minimumLength) 
        {
            // Arrays can't be smaller than zero.  We allow requesting zero-length arrays (even though
            // pooling such an array isn't valuable) as it's a valid length array, and we want the pool
            // to be usable in general instead of using `new`, even for computed lengths.
            if(minimumLength < 0) 
            {
                throw new ArgumentOutOfRangeException("minimumLength");
            }
            else if(minimumLength == 0) 
            {
                // No need for events with the empty array.  Our pool is effectively infinite
                // and we'll never allocate for rents and never store for returns.
                return s_emptyArray ?? (s_emptyArray = new T[0]);
            }

            T[] buffer = null;

            int index = Utilities.SelectBucketIndex(minimumLength);
            if(index < _buckets.Length)
            {
                // Search for an array starting at the 'index' bucket. If the bucket is empty, bump up to the
                // next higher bucket and try that one, but only try at most a few buckets.
                const int MaxBucketsToTry = 2;
                int i = index;
                do 
                {
                    // Attempt to rent from the bucket.  If we get a buffer from it, return it.
                    buffer = _buckets[i].Rent();
                    if(buffer != null) 
                    {
                        return buffer;
                    }
                }
                while(++i < _buckets.Length && i != index + MaxBucketsToTry);

                // The pool was exhausted for this buffer size.  Allocate a new buffer with a size corresponding
                // to the appropriate bucket.
                buffer = new T[_buckets[index]._bufferLength];
            }
            else 
            {
                // The request was for a size too large for the pool.  Allocate an array of exactly the requested length.
                // When it's returned to the pool, we'll simply throw it away.
                buffer = new T[minimumLength];
            }
            return buffer;
        }

        public override void Return(T[] array, bool clearArray) 
        {
            if(array == null)
            {
                throw new ArgumentNullException("array");
            }
            else if(array.Length == 0) 
            {
                // Ignore empty arrays.  When a zero-length array is rented, we return a singleton
                // rather than actually taking a buffer out of the lowest bucket.
                return;
            }

            // Determine with what bucket this array length is associated
            int bucket = Utilities.SelectBucketIndex(array.Length);

            // If we can tell that the buffer was allocated, drop it. Otherwise, check if we have space in the pool
            if(bucket < _buckets.Length) 
            {
                // Clear the array if the user requests
                if(clearArray) 
                {
                    Array.Clear(array, 0, array.Length);
                }

                // Return the buffer to its bucket.  In the future, we might consider having Return return false
                // instead of dropping a bucket, in which case we could try to return to a lower-sized bucket,
                // just as how in Rent we allow renting from a higher-sized bucket.
                _buckets[bucket].Return(array);
            }
        }
    }
    public sealed partial class DefaultArrayPool<T> : ArrayPool<T> 
    {
        /// <summary>Provides a thread-safe bucket containing buffers that can be Rent'd and Return'd.</summary>
        private sealed class Bucket 
        {
            public readonly int _bufferLength;
            private readonly T[][] _buffers;
            private readonly int _poolId;
            private int _index;

            /// <summary>
            /// Creates the pool with numberOfBuffers arrays where each buffer is of bufferLength length.
            /// </summary>
            public Bucket(int bufferLength, int numberOfBuffers, int poolId) 
            {
                _buffers = new T[numberOfBuffers][];
                _bufferLength = bufferLength;
                _poolId = poolId;
                Id = GetHashCode();
            }

            /// <summary>Gets an ID for the bucket to use with events.</summary>
            public int Id = 0;

            /// <summary>Takes an array from the bucket.  If the bucket is empty, returns null.</summary>
            public T[] Rent() 
            {
                T[][] buffers = _buffers;
                T[] buffer = null;

                // While holding the lock, grab whatever is at the next available index and
                // update the index.  We do as little work as possible while holding the spin
                // lock to minimize contention with other threads.  The try/finally is
                // necessary to properly handle thread aborts on platforms which have them.
                bool lockTaken = false, allocateBuffer = false;
                try 
                {
                    if(_index < buffers.Length)
                    {
                        //UnityEngine.Debug.Log("我是从缓存来的");
                        buffer = buffers[_index];
                        buffers[_index++] = null;
                        allocateBuffer = buffer == null;
                    }
                }
                finally 
                {

                }

                // While we were holding the lock, we grabbed whatever was at the next available index, if
                // there was one.  If we tried and if we got back null, that means we hadn't yet allocated
                // for that slot, in which case we should do so now.
                if(allocateBuffer) 
                {
                    UnityEngine.Debug.Log("new 出来的");
                    buffer = new T[_bufferLength];
                }

                return buffer;
            }

            /// <summary>
            /// Attempts to return the buffer to the bucket.  If successful, the buffer will be stored
            /// in the bucket and true will be returned; otherwise, the buffer won't be stored, and false
            /// will be returned.
            /// </summary>
            public void Return(T[] array) 
            {
                // Check to see if the buffer is the correct size for this bucket
                if(array.Length != _bufferLength)
                {
                    throw new ArgumentException("array不是来自内存池");
                }

                // While holding the spin lock, if there's room available in the bucket,
                // put the buffer into the next available slot.  Otherwise, we just drop it.
                // The try/finally is necessary to properly handle thread aborts on platforms
                // which have them.
                bool lockTaken = false;
                try 
                {
                    if(_index != 0) 
                    {
                        _buffers[--_index] = array;
                    }
                }
                finally 
                {

                }
            }
        }
    }

    public static class Utilities 
    {
        public static int SelectBucketIndex(int bufferSize) 
        {

            uint bitsRemaining = ((uint)bufferSize - 1) >> 4;

            int poolIndex = 0;
            if(bitsRemaining > 0xFFFF) 
            {
                bitsRemaining >>= 16;
                poolIndex = 16;
            }
            if(bitsRemaining > 0xFF) 
            {
                bitsRemaining >>= 8;
                poolIndex += 8;
            }
            if(bitsRemaining > 0xF)
            {
                bitsRemaining >>= 4;
                poolIndex += 4;
            }
            if(bitsRemaining > 0x3) 
            {
                bitsRemaining >>= 2;
                poolIndex += 2;
            }
            if(bitsRemaining > 0x1)
            {
                bitsRemaining >>= 1;
                poolIndex += 1;
            }

            return poolIndex + (int)bitsRemaining;
        }

        public static int GetMaxSizeForBucket(int binIndex) 
        {
            int maxSize = 16 << binIndex;
            return maxSize;
        }
    }

}

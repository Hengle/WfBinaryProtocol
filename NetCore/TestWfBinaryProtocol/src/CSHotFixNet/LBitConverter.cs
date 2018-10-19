//基础来源是NetFramework的源代码，稍作修改
using System;

public static class LBitConverter
{
    public static void GetBytes(bool value, byte[] bytes, ref int offset)
    {
        bytes[offset++] = (value ? (byte)1 : (byte)0);
    }

    public static void GetBytes(char value, byte[] bytes, ref int offset)
    {
        GetBytes((short)value, bytes, ref offset);
    }

    public unsafe static void GetBytes(short value, byte[] bytes, ref int offset)
    {
#if BIGENDIAN
        value = System.Net.IPAddress.HostToNetworkOrder(value);
#endif
        fixed (byte* b = bytes)
            *((short*)(b+offset)) = value;
        offset += 2;
    }

    public unsafe static void GetBytes(byte value, byte[] bytes, ref int offset)
    {
        fixed (byte* b = bytes)
            *((byte*)(b + offset)) = value;
        offset += 1;
    }
    public unsafe static void GetBytes(sbyte value, byte[] bytes, ref int offset)
    {
        fixed (byte* b = bytes)
            *((sbyte*)(b + offset)) = value;
        offset += 1;
    }

    public unsafe static void GetBytes(int value, byte[] bytes, ref int offset)
    {
#if BIGENDIAN
        value = System.Net.IPAddress.HostToNetworkOrder(value);
#endif
        fixed (byte* b = bytes)
            *((int*)(b+offset)) = value;
        offset += 4;
    }

    public unsafe static void GetBytes(long value, byte[] bytes, ref int offset)
    {
#if BIGENDIAN
        value = System.Net.IPAddress.HostToNetworkOrder(value);
#endif
        fixed (byte* b = bytes)
            *((long*)(b+offset)) = value;
        offset += 8;
    }
    public static void GetBytes(string value, byte[] bytes, ref int offset)
    {
        //需要提前获得字节的长度
        int length = System.Text.Encoding.UTF8.GetBytes(value, 0, value.Length, bytes, offset + 4);
        GetBytes(length, bytes, ref offset);
        offset += length;
    }

    public static void GetBytes(ushort value, byte[] bytes, ref int offset)
    {
        GetBytes((short)value, bytes, ref offset);
    }


    public static void GetBytes(uint value, byte[] bytes, ref int offset)
    {
        GetBytes((int)value, bytes, ref offset);
    }


    public static void GetBytes(ulong value, byte[] bytes, ref int offset)
    {
        GetBytes((long)value, bytes, ref offset);
    }

    public unsafe static void GetBytes(float value, byte[] bytes, ref int offset)
    {
        GetBytes(*(int*)&value, bytes, ref offset);
    }

    public unsafe static void GetBytes(double value, byte[] bytes, ref int offset)
    {
        GetBytes(*(long*)&value, bytes, ref offset);
    }

    // Converts an array of bytes into a char.  
    public static char ToChar(byte[] value,ref int startIndex)
    {
        char hr = BitConverter.ToChar(value, startIndex);
        startIndex += 2;
        return hr;
    }

    public static unsafe short ToInt16(byte[] value,ref int startIndex)
    {
        short hr = BitConverter.ToInt16(value, startIndex);
        startIndex += 2;
        return hr;
    }

    public static unsafe int ToInt32(byte[] value,ref int startIndex)
    {
        int hr = BitConverter.ToInt32(value, startIndex);
        startIndex += 4;
        return hr;
    }


    public static unsafe long ToInt64(byte[] value,ref int startIndex)
    {
        long hr = BitConverter.ToInt64(value, startIndex);
        startIndex += 8;
        return hr;
    }


    public static ushort ToUInt16(byte[] value,ref int startIndex)
    {
        ushort hr = BitConverter.ToUInt16(value, startIndex);
        startIndex += 2;
        return hr;
    }


    public static uint ToUInt32(byte[] value,ref int startIndex)
    {
        uint hr =  BitConverter.ToUInt32(value, startIndex);
        startIndex += 4;
        return hr;
    }

    public static ulong ToUInt64(byte[] value,ref int startIndex)
    {
        ulong hr = BitConverter.ToUInt64(value, startIndex);
        startIndex += 8;
        return hr;
    }

    unsafe public static float ToSingle(byte[] value,ref int startIndex)
    {
        float hr = BitConverter.ToSingle(value, startIndex);
        startIndex += 4;
        return hr;
    }

    unsafe public static double ToDouble(byte[] value, ref int startIndex)
    {
        double hr = BitConverter.ToDouble(value, startIndex);
        startIndex += 8;
        return hr;
    }


    public static String ToString(byte[] value,ref int startIndex, int length)
    {
        string hr = System.Text.Encoding.UTF8.GetString(value, startIndex, length);
        startIndex += length;
        return hr;
    }

    public static bool ToBoolean(byte[] value,ref int startIndex)
    {
        bool hr = BitConverter.ToBoolean(value, startIndex);
        startIndex++;
        return hr;
    }

}
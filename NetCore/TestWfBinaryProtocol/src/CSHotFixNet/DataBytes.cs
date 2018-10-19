using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DataBytes:PooledClassObject
{
    private byte[] Bytes = null;
    //有效长度
    private int Length = 0;
    public override void New(object param)
    {
        Length = 0;
    }
    public override void Delete()
    {
        NetworkManager.GetBytePool().Return(Bytes, true);
        Bytes = null;
        Length = 0;
    }
    public void SetBytes(int length)
    {
        Length = length;
        Bytes = NetworkManager.GetBytePool().Rent(length);
    }
    public byte[] GetBytes()
    {
        return Bytes;
    }
    public int GetLength()
    {
        return Length;
    }
    public override void DestroyClass()
    {
        PooledClassManager<DataBytes>.DeleteClass(this);
    }
}


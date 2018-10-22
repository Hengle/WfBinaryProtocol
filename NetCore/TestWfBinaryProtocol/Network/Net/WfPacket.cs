using System;
using System.Collections.Generic;

using System.Text;


public class WfPacket
{
    public ushort m_Command = 0;
    private Encoding m_Encoding = Encoding.UTF8;
    protected byte[] m_Bytes = null;
    protected int m_Offset = 0;

    public void SetEncoding(Encoding encoding)
    {
        this.m_Encoding = encoding;
    }
    public byte[] GetBytes()
    {
        return this.m_Bytes;
    }
    public int GetOffset()
    {
        return this.m_Offset;
    }
    public uint ReadUInt()
    {
        uint result = BitConverter.ToUInt32(this.m_Bytes, this.m_Offset);
        this.m_Offset += 4;
        return result;
    }
    public ulong ReadUInt64()
    {
        ulong result = BitConverter.ToUInt64(this.m_Bytes, this.m_Offset);
        this.m_Offset += 8;
        return result;
    }
    public int ReadInt()
    {
        int result = BitConverter.ToInt32(this.m_Bytes, this.m_Offset);
        this.m_Offset += 4;
        return result;
    }
    public long ReadInt64()
    {
        long result = BitConverter.ToInt64(this.m_Bytes, this.m_Offset);
        this.m_Offset += 8;
        return result;
    }
    public ushort ReadUShort()
    {
        ushort result = BitConverter.ToUInt16(this.m_Bytes, this.m_Offset);
        this.m_Offset += 2;
        return result;
    }
    public short ReadShort()
    {
        short result = BitConverter.ToInt16(this.m_Bytes, this.m_Offset);
        this.m_Offset += 2;
        return result;
    }
    public byte ReadByte()
    {
        byte result = this.m_Bytes[this.m_Offset];
        this.m_Offset++;
        return result;
    }
    public sbyte ReadSByte()
    {
        sbyte result = (sbyte)this.m_Bytes[this.m_Offset];
        this.m_Offset++;
        return result;
    }
    public bool ReadBool()
    {
        bool result = BitConverter.ToBoolean(this.m_Bytes, this.m_Offset);
        this.m_Offset++;
        return result;
    }
    public float ReadFloat()
    {
        float result = BitConverter.ToSingle(this.m_Bytes, this.m_Offset);
        this.m_Offset += 4;
        return result;
    }
    public double ReadDouble()
    {
        double result = BitConverter.ToDouble(this.m_Bytes, this.m_Offset);
        this.m_Offset += 8;
        return result;
    }
    public string ReadString()
    {
        int num = this.ReadInt();
        string result;
        if (num <= 0)
        {
            result = "";
        }
        else
        {
            try
            {
                byte[] array = new byte[num + 1];
                Array.Copy(this.m_Bytes, this.m_Offset, array, 0, num);
                array[num] = 0;
                Encoding encoding = Encoding.GetEncoding("utf-8");
                string @string = encoding.GetString(array);
                this.m_Offset += num;
                result = @string.Replace("\0", "");
            }
            catch
            {
                result = "";
            }
        }
        return result;
    }
    public string ReadString(int strLength)
    {
        string result;
        try
        {
            byte[] array = new byte[strLength + 1];
            Array.Copy(this.m_Bytes, this.m_Offset, array, 0, strLength);
            array[strLength] = 0;
            Encoding encoding = Encoding.GetEncoding("utf-8");
            string @string = encoding.GetString(array);
            this.m_Offset += strLength;
            result = @string;
        }
        catch
        {
            result = "";
        }
        return result;
    }
    public byte[] ReadBytes()
    {
        int num = this.ReadInt();
        byte[] result;
        if (num <= 0)
        {
            result = null;
        }
        else
        {
            byte[] array = new byte[num];
            Array.Copy(this.m_Bytes, this.m_Offset, array, 0, num);
            this.m_Offset += num;
            result = array;
        }
        return result;
    }
    public byte[] ReadBytes(int dataLength)
    {
        byte[] array = new byte[dataLength];
        Array.Copy(this.m_Bytes, this.m_Offset, array, 0, dataLength);
        this.m_Offset += dataLength;
        return array;
    }

    public void Write(uint i)
    {
        byte[] bytes = BitConverter.GetBytes(i);
        Array.Copy(bytes, 0, this.m_Bytes, this.m_Offset, 4);
        this.m_Offset += 4;
    }
    public void Write(int i)
    {
        byte[] bytes = BitConverter.GetBytes(i);
        Array.Copy(bytes, 0, this.m_Bytes, this.m_Offset, 4);
        this.m_Offset += 4;
    }
    public void Write(ulong i)
    {
        byte[] bytes = BitConverter.GetBytes(i);
        Array.Copy(bytes, 0, this.m_Bytes, this.m_Offset, 8);
        this.m_Offset += 8;
    }
    public void Write(ushort i)
    {
        byte[] bytes = BitConverter.GetBytes(i);
        Array.Copy(bytes, 0, this.m_Bytes, this.m_Offset, 2);
        this.m_Offset += 2;
    }
    public void Write(short i)
    {
        byte[] bytes = BitConverter.GetBytes(i);
        Array.Copy(bytes, 0, this.m_Bytes, this.m_Offset, 2);
        this.m_Offset += 2;
    }
    public void Write(byte i)
    {
        byte[] bytes = BitConverter.GetBytes((short)i);
        Array.Copy(bytes, 0, this.m_Bytes, this.m_Offset, 1);
        this.m_Offset++;
    }
    public void Write(sbyte i)
    {
        byte[] bytes = BitConverter.GetBytes((short)i);
        Array.Copy(bytes, 0, this.m_Bytes, this.m_Offset, 1);
        this.m_Offset++;
    }
    public void Write(bool b)
    {
        byte[] bytes = BitConverter.GetBytes(b);
        Array.Copy(bytes, 0, this.m_Bytes, this.m_Offset, 1);
        this.m_Offset++;
    }
    public void Write(float f)
    {
        byte[] bytes = BitConverter.GetBytes(f);
        Array.Copy(bytes, 0, this.m_Bytes, this.m_Offset, 4);
        this.m_Offset += 4;
    }
    public void Write(double d)
    {
        byte[] bytes = BitConverter.GetBytes(d);
        Array.Copy(bytes, 0, this.m_Bytes, this.m_Offset, 8);
        this.m_Offset += 8;
    }
    public void Write(string str)
    {
        Encoding encoding = Encoding.GetEncoding("utf-8");
        byte[] bytes = encoding.GetBytes(str);
        this.Write(bytes.Length);
        Array.Copy(bytes, 0, this.m_Bytes, this.m_Offset, bytes.Length);
        this.m_Offset += bytes.Length;
    }
    public bool Write(string str, int length)
    {
        byte[] bytes = this.m_Encoding.GetBytes(str);
        bool result;
        if (bytes.Length > length)
        {
            result = false;
        }
        else
        {
            Array.Copy(bytes, 0, this.m_Bytes, this.m_Offset, bytes.Length);
            this.m_Offset += bytes.Length;
            if (bytes.Length < length)
            {
                for (int i = 0; i < length - bytes.Length; i++)
                {
                    this.m_Bytes[i + this.m_Offset] = 0;
                }
                this.m_Offset += length - bytes.Length;
            }
            result = true;
        }
        return result;
    }
    public void Write(byte[] bytes, int startIndex, int bytesCount)
    {
        Array.Copy(bytes, startIndex, this.m_Bytes, this.m_Offset, bytesCount);
        this.m_Offset += bytesCount;
    }


/// ///////////////////////////////////////////////简单的分割线///////////////////////////////////////////





    public WfPacket(int msgType, int defaultSize = 10240)
    {
        this.m_Bytes = new byte[defaultSize];
        Write((ushort)0);
        Write((ushort)msgType);
        Write((uint)0);  //服务器需要的一个字段（PacketExLen）
        m_Command = (ushort)msgType;


    }
    public WfPacket(byte[] bytes)
    {
        this.m_Bytes = bytes;
    }
    public static int GetHeaderLength()
    {
        return 2 + 2 + 4;
    }
    public void SetHeadLength()
    {
        byte[] bytes = BitConverter.GetBytes(GetOffset());
        Array.Copy(bytes, 0, m_Bytes, 0, 2);
    }
    public bool ReadHeader(ref ushort msgType, ref ushort msgLength)
    {
        m_Offset = 0;
        msgLength = ReadUShort();
        msgType = ReadUShort();
        uint server_use = ReadUInt();
        return true;
    }
    public static bool ParseHeader(byte[] packetHeader, ref ushort msgType, ref ushort msgLength)
    {
        bool result;
        if (packetHeader.Length < WfPacket.GetHeaderLength())
        {
            result = false;
        }
        else
        {
            msgLength = BitConverter.ToUInt16(packetHeader, 0);
            msgType = BitConverter.ToUInt16(packetHeader, 2);
            uint server_use = BitConverter.ToUInt32(packetHeader, 4);
            result = true;
        }
        return result;
    }
}

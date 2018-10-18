using System;
using System.Collections.Generic;

using System.Text;

public class WfPacket : PooledClassObject
{
    public ushort m_Command = 0;

    protected static byte[] s_WriteBytes = new byte[1024 * 20];
    protected DataBytes m_ReadBytes = null;
    protected int m_Offset = 0;


    public override void DestroyClass()
    {
        if (m_ReadBytes != null)
        {
            m_ReadBytes.DestroyClass();
            m_ReadBytes = null;
        }
        m_Offset = 0;
        PooledClassManager<WfPacket>.DeleteClass(this);
    }
    //将大缓冲的字节转移到此包
    public void Swap()
    {
        m_ReadBytes = PooledClassManager<DataBytes>.CreateClass();
        m_ReadBytes.SetBytes(m_Offset);
        Array.Copy(s_WriteBytes, 0, m_ReadBytes.GetBytes(), 0, m_Offset);

    }

    public  void InitWrite(ushort msgType)
    {
        Array.Clear(s_WriteBytes, 0, s_WriteBytes.Length);
        m_Offset = 0;
        Write((ushort)0);
        Write((ushort)msgType);
        Write((uint)0);  //服务器需要的一个字段（PacketExLen）
        m_Command = msgType;
    }
    public byte[] GetReadBytes()
    {
        return m_ReadBytes.GetBytes();
    }

    public int GetOffset()
    {
        return m_Offset;
    }

    public  void InitRead(DataBytes data)
    {
        m_Offset = 0;
        m_ReadBytes = data;
    }

    public static int GetHeaderLength()
    {
        //struct PacketHeader
        //{
        //    PacketLength m_Length;
        //    //uint16	m_CRC16_CCITT;		// crc校验
        //    //uint16	m_Length;			// 消息长度,包括PacketHeader的长度	uint16
        //    PacketCommand m_Command;        // 消息号			uint16
        //    PacketParam m_PacketParam;  // 消息扩展长度		uint32
        //};
        return 2 + 2 + 4;
    }
    public void SetHeadLength()
    {
        int offset = 0;
        LBitConverter.GetBytes((ushort)GetOffset(), m_ReadBytes.GetBytes(), ref offset);
    }
    public bool ReadHeader(ref ushort msgType, ref ushort msgLength)
    {
        m_Offset = 0;
        msgLength = ReadUShort();
        msgType = ReadUShort();
        uint server_use = ReadUInt();
        m_Command = msgType;
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
            int offset = 0;
            msgLength = LBitConverter.ToUInt16(packetHeader, ref offset);
            msgType = LBitConverter.ToUInt16(packetHeader, ref offset);
            uint server_use = LBitConverter.ToUInt32(packetHeader, ref offset);
            result = true;
        }
        return result;
    }

    public uint ReadUInt()
    {
        return LBitConverter.ToUInt32(m_ReadBytes.GetBytes(), ref m_Offset);
    }
    public ulong ReadUInt64()
    {
        return LBitConverter.ToUInt64(m_ReadBytes.GetBytes(), ref m_Offset);
    }
    public int ReadInt()
    {
        return LBitConverter.ToInt32(m_ReadBytes.GetBytes(), ref m_Offset);
    }
    public long ReadInt64()
    {
        return LBitConverter.ToInt64(m_ReadBytes.GetBytes(), ref m_Offset);
    }
    public ushort ReadUShort()
    {
        return LBitConverter.ToUInt16(m_ReadBytes.GetBytes(), ref m_Offset);
    }
    public short ReadShort()
    {
        return LBitConverter.ToInt16(m_ReadBytes.GetBytes(), ref m_Offset);
    }
    public byte ReadByte()
    {
        byte result = m_ReadBytes.GetBytes()[m_Offset];
        m_Offset++;
        return result;
    }
    public sbyte ReadSByte()
    {
        sbyte result = (sbyte)m_ReadBytes.GetBytes()[m_Offset];
        m_Offset++;
        return result;
    }
    public bool ReadBool()
    {
        return LBitConverter.ToBoolean(m_ReadBytes.GetBytes(), ref m_Offset);
    }
    public float ReadFloat()
    {
        return LBitConverter.ToSingle(m_ReadBytes.GetBytes(), ref m_Offset);
    }
    public double ReadDouble()
    {
        return LBitConverter.ToDouble(m_ReadBytes.GetBytes(), ref m_Offset);
    }
    public string ReadString()
    {
        int num = ReadInt();
        return ReadString(num);
    }
    public string ReadString(int strLength)
    {
        return LBitConverter.ToString(m_ReadBytes.GetBytes(), ref m_Offset, strLength);
    }


    public void Write(uint i)
    {
        LBitConverter.GetBytes(i, s_WriteBytes, ref m_Offset);
    }
    public void Write(int i)
    {
        LBitConverter.GetBytes(i, s_WriteBytes, ref m_Offset);
    }
    public void Write(ulong i)
    {
        LBitConverter.GetBytes(i, s_WriteBytes, ref m_Offset);
    }
    public void Write(long i)
    {
        LBitConverter.GetBytes(i, s_WriteBytes, ref m_Offset);
    }
    public void Write(ushort i)
    {
        LBitConverter.GetBytes(i, s_WriteBytes, ref m_Offset);
    }
    public void Write(short i)
    {
        LBitConverter.GetBytes(i, s_WriteBytes, ref m_Offset);
    }
    public void Write(byte i)
    {
        LBitConverter.GetBytes(i, s_WriteBytes, ref m_Offset);
    }
    public void Write(sbyte i)
    {
        LBitConverter.GetBytes(i, s_WriteBytes, ref m_Offset);
    }
    public void Write(bool i)
    {
        LBitConverter.GetBytes(i, s_WriteBytes, ref m_Offset);
    }
    public void Write(float i)
    {
        LBitConverter.GetBytes(i, s_WriteBytes, ref m_Offset);
    }
    public void Write(double i)
    {
        LBitConverter.GetBytes(i, s_WriteBytes, ref m_Offset);
    }
    public void Write(String str)
    {
        LBitConverter.GetBytes(str, s_WriteBytes, ref m_Offset);
    }
}

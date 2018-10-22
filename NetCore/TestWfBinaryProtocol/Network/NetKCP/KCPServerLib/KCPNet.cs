using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using KCP.Server;
using KCP.Common;
using GameServer;

public class KCPNet
{

    private static KCP.Server.KCPServer m_Net = null;
    private static Thread m_Thread = null;
    private static ArrayPool<byte> BytePool = null;
    private static bool m_bDestory = false;
    public static void SetHandshakeDelay(int delay)
    {
        UdpLibConfig.HandshakeDelay = delay;
    }
    public static void SetHandshakeRetry(int count)
    {
        UdpLibConfig.HandshakeRetry = count;
    }
    public static void SetUseBytePool(bool bpool)
    {
        UdpLibConfig.UseBytePool = bpool;
    }





    public static void Init()
    {
        string myIp = "127.0.0.1";
        ushort myProt = 40000;


        m_bDestory = false;
        InitBytePool();
        m_Net = new KCP.Server.KCPServer(myIp, myProt);

        m_Net.NewClientSession += server_NewClientSession;
        m_Net.CloseClientSession += server_CloseClientSession;
        m_Net.RecvData += server_RecvData;
        for (int i = 1; i < 10000; i++)
        {
            m_Net.AddClientKey((uint)i, i);
        }
        PacketRegister.Init(null);
        m_Net.Start();
        Log.print("启动监听{0}:{1}成功", myIp, myProt);
        while (true)
        {
            m_Net.Update();
        }

    }


    private static void InitBytePool()
    {
        if (BytePool == null)
        {
            if (UdpLibConfig.UseBytePool)
            {
                BytePool = ArrayPool<byte>.Create(8 * 1024, 50);
            }
            else
            {
                BytePool = ArrayPool<byte>.System();
            }
            KCPLib.BufferAlloc = (size) =>
            {
                return BytePool.Rent(size);
            };
            KCPLib.BufferFree = (buf) =>
            {
                BytePool.Return(buf);
            };
        }
    }

    static void server_RecvData(ClientSession session, byte[] data, int offset, int size)
    {
        //byte cmd = data[offset];
        //offset++;
        //收到马上转发
        //string s = System.Text.Encoding.UTF8.GetString(data, offset + 4, size - 4);
        //Console.WriteLine("Recv From:" + session.NetIndex.ToString() + " " + session.EndPoint.ToString() + " data:" + s);
        //session.Send(s);
        UnityEngine.Debug.Log("server_RecvData");
        //读4个字节的Key校验
        uint key = BitConverter.ToUInt32(data, offset);
        int length = data.Length - offset - 4;
        byte[] msg = new byte[length];
        Array.Copy(data, offset + 4, msg, 0, length);
        WfPacket item = new WfPacket(msg);
        //向逻辑层抛
        KCPPacketHandlerMgr.GetInstance().ProcessPacket(session, item);

    }
    public static void SendPacket(ClientSession session,WfPacket packet)
    {
        byte[] data = packet.GetBytes();
        session.Send(data);
    }
    static void server_NewClientSession(ClientSession session, byte[] data, int offset, int size)
    {
        //int d = BitConverter.ToInt32(data, offset);
        Console.WriteLine("New Client:" + session.NetIndex.ToString() + " " + session.EndPoint.ToString());// + " data:" + d.ToString());
        //session.Send(new byte[1]);
    }

    static void server_CloseClientSession(ClientSession session)
    {
        Console.WriteLine("Close Client:" + session.NetIndex.ToString());
    }
}




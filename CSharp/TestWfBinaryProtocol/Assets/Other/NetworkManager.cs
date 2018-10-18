using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum NetworkProtol
{
    Tcp,
    Kcp,
    Http
}
public class NetworkManager
{
    private static ArrayPool<byte> m_BytePool = null;
    private static bool m_bUseBytePool = true;


    private static ClientNet m_TcpNet;
    //private static KCPNet m_KcpNet;
    public static ArrayPool<byte> GetBytePool()
    {
        return m_BytePool;
    }
    public static void Init()
    {
        if (m_bUseBytePool)
        {
            m_BytePool = ArrayPool<byte>.Create(8 * 1024, 50);
        }
        else
        {
            m_BytePool = ArrayPool<byte>.System();
        }
    }

    public static bool GetEnableDebug(NetworkProtol np)
    {
        if (np == NetworkProtol.Tcp)
        {
            return m_TcpNet.GetEnableDebug();
        }
        return false;
    }

    public static void StartNetwork(NetworkProtol np)
    {
        if (np == NetworkProtol.Tcp)
        {
            m_TcpNet = new ClientNet();
        }
        else if (np == NetworkProtol.Kcp)
        {
            //m_KcpNet = new KCPNet();
        }
    }
    public static void SendPacket(NetworkProtol np, WfPacket pak)
    {
        pak.Swap();
        if (np == NetworkProtol.Tcp)
        {
            m_TcpNet.SendPacket(pak);
        }
        else if (np == NetworkProtol.Kcp)
        {
            //m_KcpNet.SendPacket(pak);
        }
    }
    public static WfPacket CreatePacket()
    {
        return PooledClassManager<WfPacket>.CreateClass();
    }
    public static void SendPacket(NetworkProtol np, ISerializePacket msg, ushort msgType)
    {
        WfPacket pak = PooledClassManager<WfPacket>.CreateClass();
        pak.InitWrite(msgType);
        msg.Serialize(pak);
        pak.Swap();
        if (np == NetworkProtol.Tcp)
        {
            SendPacket(np, pak);
        }
        else if (np == NetworkProtol.Kcp)
        {
            //WfPacket pak = new WfPacket(msgType, 548);
            SendPacket(np, pak);
        }
    }
    //自己给自己发消息，用于测试
    public static void SendPacketToMe(NetworkProtol np, ISerializePacket msg, ushort msgType)
    {
        WfPacket pak = PooledClassManager<WfPacket>.CreateClass();
        pak.InitWrite(msgType);
        msg.Serialize(pak);
        pak.Swap();
        if (np == NetworkProtol.Tcp)
        {
            GameDll.PacketHandlerManager.ProcessPacket(pak);
        }
        else if (np == NetworkProtol.Kcp)
        {
            //KCPPacketHandlerMgr.GetInstance().ProcessPacket(pak);
        }
        pak.DestroyClass();
        pak = null;
    }

    public static void SetEnableDebug(NetworkProtol np, bool debug)
    {
        if (np == NetworkProtol.Tcp)
        {
            m_TcpNet.SetEnableDebug(debug);
        }
    }
    public static void Update()
    {
        if (m_TcpNet != null)
        {
            TestNetStateChange();
            m_TcpNet.Update();
        }
    }
    public static void Destroy()
    {

    }

    public static bool IsNetRun(NetworkProtol np)
    {
        if (np == NetworkProtol.Tcp)
        {
            return m_TcpNet != null && m_TcpNet.IsNetRun();
        }
        else if (np == NetworkProtol.Kcp)
        {
            return false; //m_KcpNet != null && m_KcpNet.IsNetRun();
        }
        return false;
    }

    public static void Shutdown(NetworkProtol np)
    {
        if (np == NetworkProtol.Tcp)
        {
            if (m_TcpNet != null)
            {
                m_TcpNet.Shutdown();
            }
        }
        else if (np == NetworkProtol.Kcp)
        {
            //if(m_KcpNet!= null)
            //{
            //    m_KcpNet.Shutdown();
            //}
        }
    }
    public static void InitTcpNet(int type)
    {
        if (m_TcpNet != null)
        {
            m_TcpNet.Init((NetConSvrType)type);
        }
    }
    public static void InitKcpNet(string ip, ushort port, uint index, int key)
    {
        //if(m_KcpNet!= null)
        //{
        //    m_KcpNet.Init(ip, port, index, key);
        //}
    }
    public static void ConnectTcpNet(string ip, int port)
    {
        if (m_TcpNet != null)
        {
            m_TcpNet.Connect(ip, port);
        }
    }
    public static int GetTcpNetConSvrType()
    {
        if (m_TcpNet != null)
        {
            return (int)m_TcpNet.GetNetConSvrType();
        }
        return 0;
    }
    public static bool IsConnectState(NetworkProtol np, int state)
    {
        if (np == NetworkProtol.Tcp)
        {
            return m_TcpNet != null && m_TcpNet.isConnectState(state);
        }
        else if (np == NetworkProtol.Kcp)
        {
            return false; // m_KcpNet != null && m_KcpNet.isConnectState(state);
        }
        return false;
    }


    public static void SetKcpHandShakeDelay(int delay)
    {
        //if(m_KcpNet != null)
        //{
        //    m_KcpNet.SetHandshakeDelay(delay);
        //}
    }
    public static void SetkcpHandShakeRetry(int count)
    {
        //if (m_KcpNet != null)
        //{
        //    m_KcpNet.SetHandshakeRetry(count);
        //}
    }
    public static string GetServerIP(NetworkProtol np)
    {
        if (np == NetworkProtol.Tcp)
        {
            return m_TcpNet.GetServerIP();
        }
        else if (np == NetworkProtol.Kcp)
        {
            return "";// m_KcpNet.GetServerIP();
        }
        return "";
    }
    public static int GetServerPort(NetworkProtol np)
    {
        if (np == NetworkProtol.Tcp)
        {
            return m_TcpNet.GetServerPort();
        }
        else if (np == NetworkProtol.Kcp)
        {
            return 0;// m_KcpNet.GetServerPort();
        }
        return 0;
    }

    public static uint GetKcpNetIndex()
    {
        return 0;// m_KcpNet.GetNetIndex();
    }

    public static int GetKcpNetKey()
    {
        return 0;// m_KcpNet.GetKey();
    }

    public static void TestNetStateChange()
    {
        if (m_TcpNet != null)
        {
            if (m_TcpNet.m_bNetStateChanged)
            {
                m_TcpNet.m_bNetStateChanged = false;
                //GameDll.CGameProcedure.s_EventManager.OnNetStateChanged.SafeInvoke((int)NetworkProtol.Tcp, m_TcpNet.GetConnectState());
            }
        }
    }
}


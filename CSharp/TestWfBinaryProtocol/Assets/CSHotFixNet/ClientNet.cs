using System;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public enum NetConSvrType
{
    none,
    loginSvr,
    gameSvr
}


public  class ClientNet
{
    public enum EConnectState
    {
        PostConnecting = 0,
        Connecting,
        Connected,
        Disconnected
    }
    private  EConnectState m_ConnectState = EConnectState.Disconnected;
    private  string m_strServerIP = null;
    private  int m_nServerPort = 0;
    private  Socket m_Socket = null;
    private  Queue<WfPacket> m_RecvQueue = new Queue<WfPacket>();
    private  Queue<WfPacket> m_SendQueue = new Queue<WfPacket>();
    private  object m_PrivateLockObject = new object();
    private  Thread m_Thread;
    private  NetworkReachability m_CurNetwork;
    private  uint m_uNextCheckTime = 0;
    private  bool m_bEnableDebug = false;
    public bool m_bNetStateChanged = false;
    public  void SetEnableDebug(bool debug)
    {
        m_bEnableDebug = debug;
    }
    public  bool GetEnableDebug()
    {
        return m_bEnableDebug;
    }

    public  bool isConnectState(int id)
    {
        return (int)m_ConnectState == id;
    }
    public int GetConnectState()
    {
        return (int)m_ConnectState;
    }
    public  string GetServerIP()
    {
        return m_strServerIP;
    }
    public  int GetServerPort()
    {
        return m_nServerPort;
    }
    public  NetConSvrType m_conType = NetConSvrType.none;
    public  NetConSvrType GetNetConSvrType()
    {
        return m_conType;
    }

    public  void Init(NetConSvrType type)
    {
        m_conType = type;
        

        if (m_Thread == null)
        {
            m_Thread = new Thread(new ThreadStart(WfNetworkThread));
            m_Thread.Name = "WfNetworkThread";
            m_Thread.IsBackground = true;
            m_Thread.Start();
        }

    }
    public  bool IsNetRun()
    {
        return m_Thread != null;
    }
    public  void Shutdown()
    {
        Connect(null, 0);
    }
    public  void SendPacket(WfPacket msg)
    {
        object privateLockObject;
        Monitor.Enter(privateLockObject = m_PrivateLockObject);
        try
        {
            m_SendQueue.Enqueue(msg);
        }
        finally
        {
            Monitor.Exit(privateLockObject);
        }
    }
    public  void Connect(string ip, int port)
    {
        m_strServerIP = ip;
        m_nServerPort = port;
        object privateLockObject;
        Monitor.Enter(privateLockObject = m_PrivateLockObject);
        try
        {
            m_ConnectState = EConnectState.PostConnecting;
            //GameDll.CGameProcedure.s_EventManager.OnNetStateChanged.SafeInvoke((int)NetworkProtol.Tcp, (int)m_ConnectState);

            m_CurNetwork = Application.internetReachability;
            m_uNextCheckTime = (uint)UnityEngine.Time.realtimeSinceStartup + 1000u;
        }
        finally
        {
            Monitor.Exit(privateLockObject);
        }
        Debug.Log(string.Concat(new object[]{ "Connect:",ip,":",port," internettype:", m_CurNetwork }));
    }
    public  void Update()
    {
        if (!IsNetRun())
            return;
        if (m_ConnectState == EConnectState.Connected)
        {
            if (m_uNextCheckTime < (int)UnityEngine.Time.realtimeSinceStartup)
            {
                m_uNextCheckTime = (uint)UnityEngine.Time.realtimeSinceStartup + 1000u;
                //这里只能检测客户端网络环境，无法反应是否已经掉线。
                if (m_CurNetwork != Application.internetReachability)
                {
                    Shutdown();
                    Debug.Log("internetReachability Network changed");
                }
            }
            object privateLockObject;
            Monitor.Enter(privateLockObject = m_PrivateLockObject);
            try
            {
                while (m_RecvQueue.Count > 0)
                {
                    WfPacket packet = m_RecvQueue.Dequeue();
                    GameDll.PacketHandlerManager.ProcessPacket(packet);
                    packet.DestroyClass();
                }
                //对消息进行一个分帧优化处理
                //int count = m_RecvQueue.Count;
                //if (count >= 10)
                //{
                //    for (int i = 0; i < 10; ++i)
                //    {
                //        //我们只要先收到的10条消息在这帧处理
                //        WfPacket packet = m_RecvQueue.Dequeue();
                //        PacketHandlerMgr.GetInstance().ProcessPacket(packet);
                //    }
                //}
                //else
                //{
                //    for (int i = 0; i < count; ++i)
                //    {
                //        WfPacket packet = m_RecvQueue.Dequeue();
                //        PacketHandlerMgr.GetInstance().ProcessPacket(packet);
                //    }
                //}
            }
            finally
            {
                Monitor.Exit(privateLockObject);
            }
        }

    }
    private  string RecvAll(ref HeaderBytes msgHeader,ref DataBytes msgData, ref int nRecved, ref bool bWaiting)
    {
        string result;
        if (msgData == null)
        {
            SocketError socketError;
            int recveNum = m_Socket.Receive(msgHeader.Bytes, nRecved, msgHeader.Bytes.Length - nRecved, SocketFlags.None, out socketError);
            if (recveNum < 0)
            {
                result = "消息头小于0";
                return result;
            }
            if (socketError != SocketError.Success && socketError != SocketError.WouldBlock)
            {
                result = "网络错误:" + socketError.ToString();
                m_ConnectState = EConnectState.PostConnecting;
                m_bNetStateChanged = true;
                //GameDll.CGameProcedure.s_EventManager.OnNetStateChanged.SafeInvoke((int)NetworkProtol.Tcp, (int)m_ConnectState);
                return result;
            }
            if (recveNum == 0)
            {
                bWaiting = true;
                result = null;
                return result;
            }
            nRecved += recveNum;
            if (nRecved == msgHeader.Bytes.Length)
            {
                ushort msgType = 0;
                ushort msgLength = 0;
                if (!WfPacket.ParseHeader(msgHeader.Bytes, ref msgType, ref msgLength))
                {
                    result = string.Concat(new object[]
                    {
                            "error ParseHeader type:",
                            msgType,
                            "len:",
                            msgLength
                    });
                    return result;
                }
                if (msgLength < msgHeader.Bytes.Length)
                {
                    result = string.Concat(new object[]
                    {
                        "error ParseHeader < msglen:",msgLength, "headerLength:",msgHeader.Bytes.Length
                    });
                    return result;
                }
                if (msgLength == msgHeader.Bytes.Length)
                {
                    //这里虽然只有一个是一个很简单的只带有消息头的消息，例如心跳，但是我依然用了完整消息大小
                    msgData = PooledClassManager<DataBytes>.CreateClass();
                    msgData.SetBytes(msgLength+1);
                    Array.Copy(msgHeader.Bytes, 0, msgData.GetBytes(), 0, msgHeader.Bytes.Length);
                    WfPacket item = PooledClassManager<WfPacket>.CreateClass();
                    item.InitRead(msgData);
                    msgData = null;
                    nRecved = 0;
                    object privateLockObject;
                    Monitor.Enter(privateLockObject = m_PrivateLockObject);
                    try
                    {
                        m_RecvQueue.Enqueue(item);
                    }
                    finally
                    {
                        Monitor.Exit(privateLockObject);
                    }
                }
                else
                {
                    msgData = PooledClassManager<DataBytes>.CreateClass();
                    msgData.SetBytes(msgLength);
                    Array.Copy(msgHeader.Bytes, 0, msgData.GetBytes(), 0, msgHeader.Bytes.Length);
                    nRecved = msgHeader.Bytes.Length;
                }
            }
        }
        if (msgData != null)
        {
            SocketError socketError;
            int recveNum = m_Socket.Receive(msgData.GetBytes(), nRecved, msgData.GetLength() - nRecved, SocketFlags.None, out socketError);
            //Debug.Log("底层函数接收数据：" + socketError.ToString());
            if (recveNum < 0)
            {
                result = "ReceiveData < 0";
                return result;
            }
            if (socketError != SocketError.Success && socketError != SocketError.WouldBlock)
            {
                result = "ReceiveData Failed";
                return result;
            }
            if (recveNum == 0)
            {
                bWaiting = true;
                result = null;
                return result;
            }
            nRecved += recveNum;
            if (nRecved > msgData.GetLength())
            {
                result = "ReceiveData IO error";
                return result;
            }
            if (nRecved == msgData.GetLength())
            {
                WfPacket item = PooledClassManager<WfPacket>.CreateClass();
                item.InitRead(msgData);
                msgData = null;
                nRecved = 0;
                object privateLockObject;
                Monitor.Enter(privateLockObject = m_PrivateLockObject);
                try
                {
                    m_RecvQueue.Enqueue(item);
                }
                finally
                {
                    Monitor.Exit(privateLockObject);
                }
            }
            else
            {
                bWaiting = true;
            }
        }
        result = null;
        return result;
    }
    private  string SendAll(WfPacket msgSending, ref int nSended)
    {
        SocketError socketError;
        int sendNum = m_Socket.Send(msgSending.GetReadBytes(), nSended, msgSending.GetOffset() - nSended, SocketFlags.None, out socketError);
        string result;
        if (sendNum < 0)
        {
            result = "SendData < 0";
        }
        else
        {
            if (socketError != SocketError.Success && socketError != SocketError.WouldBlock)
            {
                result = "SendData Failed";
            }
            else
            {
                nSended += sendNum;
                if (nSended > msgSending.GetOffset())
                {
                    result = "SendData IO error";
                }
                else
                {
                    result = null;
                }
            }
        }
        //Debug.Log("底层函数发送数据给服务器："+socketError.ToString());
        return result;
    }
    private  void ConnectServer()
    {
        object privateLockObject;
        Monitor.Enter(privateLockObject = m_PrivateLockObject);
        try
        {
            m_ConnectState = EConnectState.Connecting;
            //GameDll.CGameProcedure.s_EventManager.OnNetStateChanged.SafeInvoke((int)NetworkProtol.Tcp, (int)m_ConnectState);
        }
        finally
        {
            Monitor.Exit(privateLockObject);
        }
        if (m_Socket != null)
        {
            try
            {
                if (m_Socket.Connected)
                {
                    m_Socket.Shutdown(SocketShutdown.Both);
                    m_Socket.Disconnect(false);
                }
                m_Socket.Close();
            }
            catch (Exception exp)
            {
                Debug.LogError(exp.Message + exp.StackTrace);
            }
            m_Socket = null;
        }
        if (m_strServerIP != null && m_nServerPort != 0)
        {
            if (m_Socket == null)
            {
                m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            m_Socket.Connect(m_strServerIP, m_nServerPort);
            m_Socket.Blocking = false;
        }
        Monitor.Enter(privateLockObject = m_PrivateLockObject);
        try
        {
            m_RecvQueue.Clear();
            m_SendQueue.Clear();
            if (m_ConnectState != EConnectState.PostConnecting)
            {
                if (m_Socket != null && m_Socket.Connected)
                {
                    m_ConnectState = EConnectState.Connected;
                    m_bNetStateChanged = true;
                    //GameDll.CGameProcedure.s_EventManager.OnNetStateChanged.SafeInvoke((int)NetworkProtol.Tcp, (int)m_ConnectState);
                    Debug.Log("Connect Success");
                }
                else
                {
                    m_ConnectState = EConnectState.Disconnected;
                    m_bNetStateChanged = true;
                    //GameDll.CGameProcedure.s_EventManager.OnNetStateChanged.SafeInvoke((int)NetworkProtol.Tcp, (int)m_ConnectState);
                }
            }
        }
        finally
        {
            Monitor.Exit(privateLockObject);
        }
    }
    private  void WfNetworkThread()
    {
        HeaderBytes headbytes = PooledClassManager<HeaderBytes>.CreateClass();
        DataBytes databytes = null;
        int recvedNum = 0;
        WfPacket sendPacket = null;
        int sendedNum = 0;
        while (m_Thread.IsAlive)
        {
            try
            {
                Thread.Sleep(10);
                if (m_ConnectState == EConnectState.PostConnecting)
                {
                    databytes = null;
                    recvedNum = 0;
                    if(sendPacket!=null)
                    {
                        sendPacket.DestroyClass();
                        sendPacket = null;
                    }
                    sendedNum = 0;
                    ConnectServer();
                }
                if (m_ConnectState == EConnectState.Connected)
                {
                    string errorText = null;
                    bool bWaiting = false;
                    do
                    {
                        errorText = RecvAll(ref headbytes, ref databytes, ref recvedNum, ref bWaiting);
                    }
                    while (errorText == null && !bWaiting);
                    if (errorText == null)
                    {
                        object privateLockObject;
                        Monitor.Enter(privateLockObject = m_PrivateLockObject);
                        try
                        {
                            while (m_SendQueue.Count > 0 || sendPacket != null)
                            {
                                if (sendPacket != null)
                                {
                                    errorText = SendAll(sendPacket, ref sendedNum);
                                    if (sendedNum == sendPacket.GetOffset())
                                    {
                                        sendPacket.DestroyClass();
                                        sendPacket = null;
                                    }
                                }
                                if (errorText != null || sendPacket != null || m_SendQueue.Count <= 0)
                                {
                                    break;
                                }
                                sendedNum = 0;
                                sendPacket = m_SendQueue.Dequeue();
                                sendPacket.SetHeadLength();
                            }
                        }
                        finally
                        {
                            Monitor.Exit(privateLockObject);
                        }
                    }
                    if (errorText != null)
                    {
                        Debug.LogError(errorText);
                        object privateLockObject;
                        Monitor.Enter(privateLockObject = m_PrivateLockObject);
                        try
                        {
                            if (m_ConnectState != EConnectState.PostConnecting)
                            {
                                m_ConnectState = EConnectState.Disconnected;
                                m_bNetStateChanged = true;
                                //GameDll.CGameProcedure.s_EventManager.OnNetStateChanged.SafeInvoke((int)NetworkProtol.Tcp, (int)m_ConnectState);
                            }
                        }
                        finally
                        {
                            Monitor.Exit(privateLockObject);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message + ex.StackTrace);
                object privateLockObject;
                Monitor.Enter(privateLockObject = m_PrivateLockObject);
                try
                {
                    if (m_ConnectState != EConnectState.PostConnecting)
                    {
                        m_ConnectState = EConnectState.Disconnected;
                        m_bNetStateChanged = true;
                        //GameDll.CGameProcedure.s_EventManager.OnNetStateChanged.SafeInvoke((int)NetworkProtol.Tcp, (int)m_ConnectState);
                    }
                }
                finally
                {
                    Monitor.Exit(privateLockObject);
                }
            }
        }
        m_Thread.Abort();
        m_Thread.Join();
    }
}
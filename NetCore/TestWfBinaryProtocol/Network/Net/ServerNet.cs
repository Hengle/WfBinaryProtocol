using System;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GameServer;



public class ServerNet
{
    private static int myProt = 56241;   //端口 
    private static string myIp = "127.0.0.1";
    static Socket serverSocket;
    private static object m_ClientDataLock = new object();
    static Thread m_ListenClientConnectThread = null;
    public static void Init()
    {
        myIp = "127.0.0.1";
        myProt = 40000;
        //服务器IP地址  
        IPAddress ip = IPAddress.Parse(myIp);
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.Bind(new IPEndPoint(ip, myProt));  //绑定IP地址：端口  
        serverSocket.Listen(10);    //设定最多10个排队连接请求  
        Log.print("启动监听{0}成功", serverSocket.LocalEndPoint.ToString());
        //通过Clientsoket发送数据  
        m_ListenClientConnectThread = new Thread(ListenClientConnect);
        m_ListenClientConnectThread.Start();





        Console.ReadLine();
    }

    /// <summary>  
    /// 监听客户端连接  
    /// </summary>  
    private static void ListenClientConnect()
    {
        while (m_ListenClientConnectThread.IsAlive)
        {
            Socket clientSocket = serverSocket.Accept();
            Log.print("有客户端{0}连接进来了", clientSocket.RemoteEndPoint.ToString());
            //初始化新建立连接的数据
            DP_Msg clientData = new DP_Msg();
            clientData.clientSocket = clientSocket;
            clientData.m_LastConnTime = Environment.TickCount;
            Log.print("连接的TickCount：" + Environment.TickCount);
            PacketRegister.Init(clientData);

            Thread m_ReceiveMessageThread = new Thread(ReceiveMessage);
            Thread m_SendMessageThread = new Thread(SendMessage);
            Thread m_ProcessMessageThread = new Thread(ProecssMessage);
            clientData.m_ProcessMessageThread = m_ProcessMessageThread;
            clientData.m_SendMessageThread = m_SendMessageThread;
            clientData.m_ReceiveMessageThread = m_ReceiveMessageThread;

            Thread m_TestConnectionThread = new Thread(TestConnection);

            m_TestConnectionThread.Start(clientData);
            m_SendMessageThread.Start(clientData);
            m_ProcessMessageThread.Start(clientData);
            m_ReceiveMessageThread.Start(clientData);

            //向客户端发送一个HelloWorld
            if (Program.m_MainLoop.m_UseTcp)
            {
                TestTcp.SendMsg(clientData);
            }
            else
            {
                TestUdp.SendMsg(clientData);
            }
        }
    }
    private static void TestConnection(object _data)
    {
        DP_Msg data = (DP_Msg)_data;
        int lastTest = Environment.TickCount;
        int testInterval = 1000 * 10;
        int timeOut = 1000 * 60 * 5;
        while (!data.m_bError)
        {
            Thread.Sleep(100);
            if (Environment.TickCount - lastTest > testInterval)
            {
                if (Environment.TickCount - data.m_LastConnTime > timeOut)
                {
                    Log.print("链接{0}超时未有任何响应，已被踢掉。", data.clientSocket.RemoteEndPoint.ToString());
                    data.m_ReceiveMessageThread.Abort();
                    data.m_ReceiveMessageThread.Join();

                    data.m_ProcessMessageThread.Abort();
                    data.m_ProcessMessageThread.Join();

                    data.m_SendMessageThread.Abort();
                    data.m_SendMessageThread.Join();
                    data.m_bError = true;

                    break;
                }
                lastTest = Environment.TickCount;
            }

        }
        data = null;
    }
    private static void ProecssMessage(object _data)
    {
        DP_Msg data = (DP_Msg)_data;
        Log.print("ProecssMessage>>>>>>Start");
        while (!data.m_bError)
        {
            Thread.Sleep(10);
            //lock (data.m_Lock)
            {

                Queue<WfPacket> local_RecvQueue = data.m_RecvQueue;
                Queue<WfPacket> local_SendQueue = data.m_SendQueue;
                object local_Lock = data.m_Lock;
                PacketHandlerMgr local_PacketHandlerMgr = data.m_PacketHandlerMgr;
                Socket myClientSocket = data.clientSocket;
                //Log.print("ProecssMessage>>>>>>" + local_RecvQueue.Count.ToString());
                try
                {
                    //通过clientSocket接收数据  
                    //lock (local_Lock)
                    {
                        while (local_RecvQueue.Count > 0)
                        {
                            WfPacket packet = local_RecvQueue.Dequeue();
                            local_PacketHandlerMgr.ProcessPacket(data, packet);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.print(ex.Message);
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    break;
                }


            }
        }
        Log.print("线程{0}ProecssMessage结束。", data.clientSocket.RemoteEndPoint.ToString());
    }
    private static void SendMessage(object _data)
    {
        DP_Msg data = (DP_Msg)_data;
        while (!data.m_bError)
        {
            Thread.Sleep(10);

            // lock (data.m_Lock)
            {
                Queue<WfPacket> local_RecvQueue = data.m_RecvQueue;
                Queue<WfPacket> local_SendQueue = data.m_SendQueue;
                object local_Lock = data.m_Lock;
                PacketHandlerMgr local_PacketHandlerMgr = data.m_PacketHandlerMgr;
                Socket myClientSocket = data.clientSocket;

                WfPacket msgPacket = null;
                string text = null;
                //处理发送消息

                while (local_SendQueue.Count > 0 || msgPacket != null)
                {

                    int num2 = 0;
                    if (msgPacket != null)
                    {
                        text = SendAll(data, msgPacket, ref num2);
                        if (num2 == msgPacket.GetOffset())
                        {
                            msgPacket = null;
                        }
                    }
                    if (text != null || msgPacket != null || local_SendQueue.Count <= 0)
                    {
                        break;
                    }
                    num2 = 0;
                    msgPacket = local_SendQueue.Dequeue();
                    msgPacket.SetHeadLength();
                }


            }
        }
        Log.print("线程{0}SendMessage结束。", data.clientSocket.RemoteEndPoint.ToString());
    }
    /// <summary>  
    /// 接收消息  
    /// </summary>  
    /// <param name="clientSocket"></param>  
    private static void ReceiveMessage(object _data)
    {
        DP_Msg data = (DP_Msg)_data;
        while (!data.m_bError)
        {
            Thread.Sleep(10);

            //lock (data.m_Lock)
            {
                Queue<WfPacket> local_RecvQueue = data.m_RecvQueue;
                Queue<WfPacket> local_SendQueue = data.m_SendQueue;
                object local_Lock = data.m_Lock;
                PacketHandlerMgr local_PacketHandlerMgr = data.m_PacketHandlerMgr;
                Socket myClientSocket = data.clientSocket;
                string text = null;
                bool flag = false;
                byte[] array = new byte[WfPacket.GetHeaderLength()];
                byte[] array2 = null;
                int num = 0;
                do
                {
                    text = ServerNet.RecvAll(data, ref array, ref array2, ref num, ref flag);
                    if (num != 0)
                    {
                        data.m_LastConnTime = Environment.TickCount;
                    }

                }
                while (text == null && !flag);

            }
        }
        Log.print("线程{0}ReceiveMessage结束。", data.clientSocket.RemoteEndPoint.ToString());
    }
    private static string RecvAll(DP_Msg data, ref byte[] msgHeader, ref byte[] msgData, ref int nRecved, ref bool bWaiting)
    {
        string result;
        if (msgData == null)
        {
            SocketError socketError;
            int receivedCount = data.clientSocket.Receive(msgHeader, nRecved, msgHeader.Length - nRecved, SocketFlags.None, out socketError);
            if (receivedCount < 0)
            {
                result = "ReceiveHeader < 0";
                return result;
            }
            if (socketError != SocketError.Success && socketError != SocketError.WouldBlock)
            {
                result = "ReceiveHeader Failed";
                return result;
            }
            if (receivedCount == 0)
            {
                bWaiting = true;
                result = null;
                return result;
            }
            nRecved += receivedCount;
            if (nRecved == msgHeader.Length)
            {
                ushort msgType = 0;
                ushort msgLength = 0;
                if (!WfPacket.ParseHeader(msgHeader, ref msgType, ref msgLength))
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
                if (msgLength < msgHeader.Length)
                {
                    result = string.Concat(new object[]
						{
							"error ParseHeader < msglen:",
							msgLength,
							"headerLength:",
							msgHeader.Length
						});
                    return result;
                }
                if (msgLength == msgHeader.Length)
                {
                    msgData = new byte[msgLength + 1];
                    Array.Copy(msgHeader, 0, msgData, 0, msgHeader.Length);
                    WfPacket item = new WfPacket(msgData);
                    msgData = null;
                    nRecved = 0;
                    lock (data.m_Lock)
                    {
                        data.m_RecvQueue.Enqueue(item);
                    }
                }
                else
                {
                    msgData = new byte[msgLength];
                    Array.Copy(msgHeader, 0, msgData, 0, msgHeader.Length);
                    nRecved = msgHeader.Length;
                }
            }
        }
        if (msgData != null)
        {
            SocketError socketError;
            int num = data.clientSocket.Receive(msgData, nRecved, msgData.Length - nRecved, SocketFlags.None, out socketError);
            if (num < 0)
            {
                result = "ReceiveData < 0";
                return result;
            }
            if (socketError != SocketError.Success && socketError != SocketError.WouldBlock)
            {
                result = "ReceiveData Failed";
                return result;
            }
            if (num == 0)
            {
                bWaiting = true;
                result = null;
                return result;
            }
            nRecved += num;
            if (nRecved > msgData.Length)
            {
                result = "ReceiveData IO error";
                return result;
            }
            if (nRecved == msgData.Length)
            {
                WfPacket item = new WfPacket(msgData);
                msgData = null;
                nRecved = 0;

                lock (data.m_Lock)
                {
                    data.m_RecvQueue.Enqueue(item);
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
    private static string SendAll(DP_Msg data, WfPacket msgSending, ref int nSended)
    {
        SocketError socketError;
        int num = data.clientSocket.Send(msgSending.GetBytes(), nSended, msgSending.GetOffset() - nSended, SocketFlags.None, out socketError);
        string result;
        if (num < 0)
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
                nSended += num;
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
        return result;
    }


    public static void SendPacket(DP_Msg data, WfPacket msg)
    {

        lock (data.m_Lock)
        {
            if (m_bEnableDebug)
            {
                Log.print("sending:" + msg.m_Command + " to " + data.clientSocket.RemoteEndPoint.ToString());
            }
            data.m_SendQueue.Enqueue(msg);
        }
    }

    private static bool m_bEnableDebug = false;
    public static void SetEnableDebug(bool debug)
    {
        m_bEnableDebug = debug;
    }
    public static bool GetEnableDebug()
    {
        return m_bEnableDebug;
    }

}


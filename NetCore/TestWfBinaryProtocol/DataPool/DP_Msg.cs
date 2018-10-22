using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using KCP.Server;



public class DP_Msg
{
    public Socket clientSocket;
    public int m_LastConnTime = 0;//心跳检测时间
    public Queue<WfPacket> m_RecvQueue = new Queue<WfPacket>();
    public Queue<WfPacket> m_SendQueue = new Queue<WfPacket>();
    public object m_Lock = new object();
    public PacketHandlerMgr m_PacketHandlerMgr = new PacketHandlerMgr();
    public bool m_bError = false;
    public Thread m_ReceiveMessageThread;
    public Thread m_SendMessageThread;
    public Thread m_ProcessMessageThread;
}




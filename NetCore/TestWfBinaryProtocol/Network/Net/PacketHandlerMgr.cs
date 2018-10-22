using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Net.Sockets;
using GameServer;
using UnityEngine;




public class PacketHandlerMgr
{
    public delegate void PacketHandler(DP_Msg socket, WfPacket packet);
    protected Dictionary<ushort, System.Action<DP_Msg, WfPacket>> m_PacketHandlers = new Dictionary<ushort, System.Action<DP_Msg, WfPacket>>();
    public PacketHandlerMgr()
    {
    }
    public void Register(ushort msg, System.Action<DP_Msg, WfPacket> handler)
    {
        this.m_PacketHandlers[msg] = handler;
    }
    public void ProcessPacket(DP_Msg socket, WfPacket packet)
    {
        ushort msgType = 0;
        ushort msgLength = 0;
        packet.ReadHeader(ref msgType, ref msgLength);
        if (ServerNet.GetEnableDebug())
        {
            Debug.Log("receiving:" + msgType);
        }
        System.Action<DP_Msg, WfPacket> handler = null;
        if (!m_PacketHandlers.TryGetValue(msgType, out handler))
        {
            try
            {
                handler(socket, packet);
            }
            catch (Exception ex)
            {
                Log.print(ex.Message);
            }
        }
    }
}

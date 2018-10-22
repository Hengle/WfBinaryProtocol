using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using UnityEngine;
using KCP.Server;

    public class KCPPacketHandlerMgr
    {
        private static KCPPacketHandlerMgr s_KCPPacketHandlerMgr = null;
        protected Dictionary<ushort, System.Action<ClientSession, WfPacket>> m_KCPPacketHandlers = new Dictionary<ushort, System.Action<ClientSession, WfPacket>>();
        public KCPPacketHandlerMgr()
        {
        }
        public static KCPPacketHandlerMgr GetInstance()
        {
            if (KCPPacketHandlerMgr.s_KCPPacketHandlerMgr == null)
            {
                KCPPacketHandlerMgr.s_KCPPacketHandlerMgr = new KCPPacketHandlerMgr();
            }
            return KCPPacketHandlerMgr.s_KCPPacketHandlerMgr;
        }
        public void Register(ushort msg, System.Action<ClientSession, WfPacket> handler)
        {
            m_KCPPacketHandlers[msg] = handler;
        }
        public void Unregister(ushort msg)
        {
            if (m_KCPPacketHandlers.ContainsKey(msg))
            {
                m_KCPPacketHandlers.Remove(msg);
            }
        }
        public void ProcessPacket(ClientSession socket, WfPacket packet)
        {
            ushort msgType = 0;
            ushort msgLength = 0;
            packet.ReadHeader(ref msgType, ref msgLength);
            if (ServerNet.GetEnableDebug())
            {
                Debug.Log("receive:" + Convert.ToString(msgType, 16));
            }
            System.Action<ClientSession, WfPacket> packetHandler = (System.Action<ClientSession, WfPacket>)m_KCPPacketHandlers[(ushort)msgType];
            if (packetHandler == null)
            {
                Debug.Log("msg not process=" + msgType.ToString());
            }
            else
            {
                try
                {
                    packetHandler(socket, packet);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }
    }

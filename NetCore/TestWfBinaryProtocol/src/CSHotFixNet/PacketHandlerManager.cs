using System;
using System.Collections.Generic;

using System.Text;
using System.Collections;
using UnityEngine;
namespace GameDll
{
    public class PacketHandlerManager
    {
        protected static Dictionary<ushort, System.Action<WfPacket>> m_Handlers = new Dictionary<ushort, Action<WfPacket>>();

        public static void Register(ushort msg, System.Action<WfPacket> handler)
        {
            m_Handlers[msg] = handler;
        }
        public static void Unregister(ushort msg)
        {
            if (m_Handlers.ContainsKey(msg))
            {
                m_Handlers.Remove(msg);
            }
        }
        public static Action<int,int, WfPacket> PacketHandlerMgrHF_ProcessPacket;
        public static void ProcessPacket(WfPacket packet)
        {
            ushort msgType = 0;
            ushort msgLength = 0;
            packet.ReadHeader(ref msgType, ref msgLength);
            if (NetworkManager.GetEnableDebug(NetworkProtol.Tcp))
            {
                Debug.Log("收到:" + Convert.ToString(msgType, 16));
            }
            System.Action<WfPacket> hander = null;
            if (!m_Handlers.TryGetValue(msgType, out hander))
            {

                //Debug.LogWarning("msg not find in battle, will process in hotfix" + Convert.ToString(msgType, 16));
                //PacketHandlerMgrHF_ProcessPacket.SafeInvoke((int)msgType, (int)msgLength, packet);
            }
            else
            {
                try
                {
                    hander(packet);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }

        }

    }
}
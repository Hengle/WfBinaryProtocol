using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KCP.Server;

public class TestUdp
{
    public static void Init()
    {
        //初始化消息注册
        KCPPacketHandlerMgr.GetInstance().Register(0, OnClientMsg);
    }

    private static void OnClientMsg(ClientSession socket, WfPacket pak)
    {
        int logicCount = pak.ReadInt();
        WfPacket msg = new WfPacket(0, 300);
        msg.Write((int)logicCount);
        KCPNet.SendPacket(socket, msg);
        UnityEngine.Debug.Log("Send msg" + logicCount.ToString());
    }


    public static void SendMsg(DP_Msg socket)
    {

    }
}


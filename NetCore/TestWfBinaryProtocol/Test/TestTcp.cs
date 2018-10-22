using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TestTcp
{
    public static void Init(DP_Msg data)
    {
        //初始化消息注册
        data.m_PacketHandlerMgr.Register((ushort)emPacket_Battle.em_CS_InputEvent, OnClientMsg);
    }

    private static void OnClientMsg(DP_Msg socket, WfPacket pak)
    {
        Packet_StopMove msg = new Packet_StopMove();
        msg.DeSerialize(pak);
        int objId = msg.m_ObjId;
        UnityEngine.Debug.Log("收到客户端消息，ObjId：" + objId);

        SendMsg(socket);

    }

    public static void SendMsg(DP_Msg socket)
    {
        SC_PreBattle sendPak = new SC_PreBattle();
        sendPak.datas = new List<t_PreparePlayerData>();

        for(uint i=0;i<3;++i)
        {
            t_PreparePlayerData data = new t_PreparePlayerData();
            data.m_jobid = i;
            data.m_playerid = i;
            data.m_name = "Player" + i;

            sendPak.datas.Add(data);
        }
        WfPacket sendPak2 = new WfPacket((ushort)emPacket_Battle.em_SC_PreBattle);
        sendPak.Serialize(sendPak2);
        ServerNet.SendPacket(socket, sendPak2);
        UnityEngine.Debug.Log("向客户端发送消息，SC_PreBattle");
    }
}


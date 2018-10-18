using UnityEngine;
using System.Collections;
using GameDll;
using System;

public class MessageHandler
{
    //测试用代码
    private bool m_IsReceived = false;
    public bool IsReceived()
    {
        return m_IsReceived;
    }

    public void Reg()
    {
        PacketHandlerManager.Register((ushort)emPacket_Battle.em_SC_PreBattle, ResPreBattle);

    }
    // 服务器发送给客户端
    /// <summary>
    /// WfPacket:来自服务器的原始包数据
    /// SC_PreBattle：解析包数据的包体
    /// DeSerialize：反序列化操作
    /// 说明：反序列化结束后，就可以像var playerdatas = pak.datas;使用字段了。
    /// </summary>
    /// <param name="obj"></param>
    private void ResPreBattle(WfPacket obj)
    {
        Debug.Log("ResPreBattle");
        SC_PreBattle pak = new SC_PreBattle();
        //演示提示：反序列化到SC_PreBattle类
        pak.DeSerialize(obj);
        var playerdatas = pak.datas;
        m_IsReceived = true;
    }


    //客户端发送数据给服务器
    /// <summary>
    /// Packet_StopMove包体，用于存储发送的数据
    /// emPacket_Battle.em_CS_InputEvent 消息id
    /// 包体和消息Id没有必然的联系，是需要发送和接收两端协商确定
    /// </summary>
    /// <param name="id"></param>
    public void ReqStopMove(int id)
    {
        m_IsReceived = false;
        //演示提示：这里可以使用内存池
        Packet_StopMove msg = new Packet_StopMove();
        msg.m_ObjId = id;
        NetworkManager.SendPacket(NetworkProtol.Tcp, msg, (ushort) emPacket_Battle.em_CS_InputEvent);
        Debug.Log("ReqStopMove");
    }

    public void UnReg()
    {
        PacketHandlerManager.Unregister((ushort)emPacket_Battle.em_SC_PreBattle);

    }

}

package Test;

import java.io.Console;
import java.util.ArrayList;

import org.omg.CORBA.UShortSeqHelper;

import io.netty.buffer.ByteBuf;  
import io.netty.channel.ChannelHandlerContext;  
import io.netty.channel.ChannelInboundHandlerAdapter;
import message.Packet_Battle.Packet_StopMove;
import message.Packet_Battle.t_PreparePlayerData;  
  
public class SimpleServerHandler extends ChannelInboundHandlerAdapter 
{  
  
    @Override  
    public void channelRead(ChannelHandlerContext ctx, Object msg) throws Exception 
    {  
        System.out.println("SimpleServerHandler.channelRead");  
        ByteBuf buffer = (ByteBuf) msg;  
        //读取消息头信息
        int length = buffer.readShort();
        int msgType = buffer.readUnsignedShort();
        long serverFlag =  buffer.readUnsignedInt();
        
        if (message.Packet_Battle.StructEnum.getStructEnum(msgType) != null) 
        {
			ReqStopMove(buffer);
		} 
        // 释放资源，这行很关键  
        buffer.release();  
  
        ResPreBattle(ctx);
    }  
  
    //响应客户端发送的移动消息
    private void ReqStopMove(ByteBuf buf)
    {
    	Packet_StopMove msg = Packet_StopMove.readBy(buf);
    	int objId = msg.m_ObjId;
    	System.out.println("Receve ReqStopMove:---->>>>" +"ObjId:"+objId);   
	}
    //向客户端发送指令
    private void ResPreBattle(ChannelHandlerContext ctx) 
    {
    	//填充一个数据
    	ArrayList<t_PreparePlayerData> datas = new ArrayList<>();
    	for(int i=0;i<3;++i)
    	{
    		t_PreparePlayerData playerData = new t_PreparePlayerData();
    		playerData.m_jobid = 0;
    		playerData.m_playerid = i;
    		playerData.m_name = "player" + i;
    		datas.add(playerData);
    	}
    	message.Packet_Battle.SC_PreBattle msg = new  message.Packet_Battle.SC_PreBattle();
    	msg.datas = datas;
    	msg.write(ctx);
    	msg.Flush();
    	System.out.println("Send ResPreBattle:"); 	
	}
    
    @Override  
    public void exceptionCaught(ChannelHandlerContext ctx, Throwable cause) throws Exception {  
        // 当出现异常就关闭连接  
        cause.printStackTrace();  
        ctx.close();  
    }  
  
    @Override  
    public void channelReadComplete(ChannelHandlerContext ctx) throws Exception {  
        ctx.flush();  
    }  
  
}  
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
        //��ȡ��Ϣͷ��Ϣ
        int length = buffer.readShort();
        int msgType = buffer.readUnsignedShort();
        long serverFlag =  buffer.readUnsignedInt();
        
        if (message.Packet_Battle.StructEnum.getStructEnum(msgType) != null) 
        {
			ReqStopMove(buffer);
		} 
        // �ͷ���Դ�����кܹؼ�  
        buffer.release();  
  
        ResPreBattle(ctx);
    }  
  
    //��Ӧ�ͻ��˷��͵��ƶ���Ϣ
    private void ReqStopMove(ByteBuf buf)
    {
    	Packet_StopMove msg = Packet_StopMove.readBy(buf);
    	int objId = msg.m_ObjId;
    	System.out.println("Receve ReqStopMove:---->>>>" +"ObjId:"+objId);   
	}
    //��ͻ��˷���ָ��
    private void ResPreBattle(ChannelHandlerContext ctx) 
    {
    	//���һ������
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
        // �������쳣�͹ر�����  
        cause.printStackTrace();  
        ctx.close();  
    }  
  
    @Override  
    public void channelReadComplete(ChannelHandlerContext ctx) throws Exception {  
        ctx.flush();  
    }  
  
}  
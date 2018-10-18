package message;
import java.util.ArrayList;
import message.BinaryMessage;
import io.netty.buffer.ByteBuf;
import message.BinaryMessageStruct;

public final class Packet_Login{
public static class CS_Login extends BinaryMessageStruct{
        
            public byte type()
            {
                return (byte) StructEnum.CS_Login.value;
            }
			    public static CS_Login readBy(ByteBuf buffer){
				    CS_Login ele = new CS_Login();
				    ele.read(buffer);
				    return ele;
			    }
		    
	public int m_type;
	public String m_account;
	public String m_pwd;
	public String m_equipment;
	public String m_machine;

	@Override
protected void write(){

		writeInt( m_type);
		writeString( m_account);
		writeString( m_pwd);
		writeString( m_equipment);
		writeString( m_machine);
	}
	@Override
protected void read(){

		 m_type = readInt();
		 m_account = readString();
		 m_pwd = readString();
		 m_equipment = readString();
		 m_machine = readString();
	}
}

public static class SC_LoginRst extends BinaryMessageStruct{
        
            public byte type()
            {
                return (byte) StructEnum.SC_LoginRst.value;
            }
			    public static SC_LoginRst readBy(ByteBuf buffer){
				    SC_LoginRst ele = new SC_LoginRst();
				    ele.read(buffer);
				    return ele;
			    }
		    
	public int m_rst;
	public String m_sessionid;

	@Override
protected void write(){

		writeInt( m_rst);
		writeString( m_sessionid);
	}
	@Override
protected void read(){

		 m_rst = readInt();
		 m_sessionid = readString();
	}
}

public static class CS_Register extends BinaryMessageStruct{
        
            public byte type()
            {
                return (byte) StructEnum.CS_Register.value;
            }
			    public static CS_Register readBy(ByteBuf buffer){
				    CS_Register ele = new CS_Register();
				    ele.read(buffer);
				    return ele;
			    }
		    
	public String m_account;
	public String m_pwd;

	@Override
protected void write(){

		writeString( m_account);
		writeString( m_pwd);
	}
	@Override
protected void read(){

		 m_account = readString();
		 m_pwd = readString();
	}
}

public static class SC_RegisterRst extends BinaryMessageStruct{
        
            public byte type()
            {
                return (byte) StructEnum.SC_RegisterRst.value;
            }
			    public static SC_RegisterRst readBy(ByteBuf buffer){
				    SC_RegisterRst ele = new SC_RegisterRst();
				    ele.read(buffer);
				    return ele;
			    }
		    
	public int m_rst;

	@Override
protected void write(){

		writeInt( m_rst);
	}
	@Override
protected void read(){

		 m_rst = readInt();
	}
}

public static class CS_LoginGateWay extends BinaryMessageStruct{
        
            public byte type()
            {
                return (byte) StructEnum.CS_LoginGateWay.value;
            }
			    public static CS_LoginGateWay readBy(ByteBuf buffer){
				    CS_LoginGateWay ele = new CS_LoginGateWay();
				    ele.read(buffer);
				    return ele;
			    }
		    
	public String m_sessionid;

	@Override
protected void write(){

		writeString( m_sessionid);
	}
	@Override
protected void read(){

		 m_sessionid = readString();
	}
}

public static class SC_LoginGateWayRst extends BinaryMessageStruct{
        
            public byte type()
            {
                return (byte) StructEnum.SC_LoginGateWayRst.value;
            }
			    public static SC_LoginGateWayRst readBy(ByteBuf buffer){
				    SC_LoginGateWayRst ele = new SC_LoginGateWayRst();
				    ele.read(buffer);
				    return ele;
			    }
		    
	public int m_rst;
	public int m_playerid;

	@Override
protected void write(){

		writeInt( m_rst);
		writeInt( m_playerid);
	}
	@Override
protected void read(){

		 m_rst = readInt();
		 m_playerid = readInt();
	}
}

public final static class SC_PlayerInfo extends BinaryMessage{
public static final int MsgId = 263;
        
		    @Override
		    public int getMsgId(){
			    return MsgId;
		    }
		    
			    public static SC_PlayerInfo readBy(ByteBuf buffer){
				    SC_PlayerInfo ele = new SC_PlayerInfo();
				    ele.read(buffer);
				    buffer.release();
				    return ele;
			    }
		    
	public int m_playerid;
	public String m_nickname;
	public long m_money;

	@Override
protected void write(){

		writeInt( m_playerid);
		writeString( m_nickname);
		writeLong( m_money);
	}
	@Override
protected void read(){

		 m_playerid = readInt();
		 m_nickname = readString();
		 m_money = readLong();
	}
}

    public enum StructEnum
    {
        CS_Login(1),
SC_LoginRst(2),
CS_Register(3),
SC_RegisterRst(4),
CS_LoginGateWay(5),
SC_LoginGateWayRst(6),
;
        private int value;
        StructEnum(int value)
        {
            this.value = value;
        }

        public static StructEnum getStructEnum(int value)
        {
            for (StructEnum en : StructEnum.values())
            {
                if (en.value == value)
                return en;
            }
            return null;
        }
    }

}
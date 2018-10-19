package message;
import java.util.ArrayList;
import message.BinaryMessage;
import io.netty.buffer.ByteBuf;
import message.BinaryMessageStruct;

public final class Packet_Battle{
public static class t_PreparePlayerData extends BinaryMessageStruct{
        
            public byte type()
            {
                return (byte) StructEnum.t_PreparePlayerData.value;
            }
			    public static t_PreparePlayerData readBy(ByteBuf buffer){
				    t_PreparePlayerData ele = new t_PreparePlayerData();
				    ele.read(buffer);
				    return ele;
			    }
		    
	public int m_playerid;
	public String m_name;
	public int m_jobid;

	@Override
protected void write(){

		writeInt( m_playerid);
		writeString( m_name);
		writeInt( m_jobid);
	}
	@Override
protected void read(){

		 m_playerid = readInt();
		 m_name = readString();
		 m_jobid = readInt();
	}
}

public final static class SC_PreBattle extends BinaryMessage{
public static final int MsgId = 3841;
        
		    @Override
		    public int getMsgId(){
			    return MsgId;
		    }
		    
			    public static SC_PreBattle readBy(ByteBuf buffer){
				    SC_PreBattle ele = new SC_PreBattle();
				    ele.read(buffer);
				    buffer.release();
				    return ele;
			    }
		    
	public ArrayList<t_PreparePlayerData> datas = new ArrayList<>();

	@Override
protected void write(){

		int _TempSize = 0;
						_TempSize = datas.size();
						writeInt( _TempSize );
						for(t_PreparePlayerData _var : datas)
						{
							_var.write();
						}
				
	}
	@Override
protected void read(){

		int _TempSize = 0;
						_TempSize =  readInt();
						for( int i =0;i< _TempSize;++i)
						{
							t_PreparePlayerData _var = new t_PreparePlayerData(); 
			_var.read();
							datas.add(_var);
						}
				
	}
}

public final static class SC_PrepareOk extends BinaryMessage{
public static final int MsgId = 3843;
        
		    @Override
		    public int getMsgId(){
			    return MsgId;
		    }
		    
			    public static SC_PrepareOk readBy(ByteBuf buffer){
				    SC_PrepareOk ele = new SC_PrepareOk();
				    ele.read(buffer);
				    buffer.release();
				    return ele;
			    }
		    
	public ArrayList<t_PreparePlayerData> datas = new ArrayList<>();

	@Override
protected void write(){

		int _TempSize = 0;
						_TempSize = datas.size();
						writeInt( _TempSize );
						for(t_PreparePlayerData _var : datas)
						{
							_var.write();
						}
				
	}
	@Override
protected void read(){

		int _TempSize = 0;
						_TempSize =  readInt();
						for( int i =0;i< _TempSize;++i)
						{
							t_PreparePlayerData _var = new t_PreparePlayerData(); 
			_var.read();
							datas.add(_var);
						}
				
	}
}

public final static class CS_LoadSceneOk extends BinaryMessage{
public static final int MsgId = 3842;
        
		    @Override
		    public int getMsgId(){
			    return MsgId;
		    }
		    
			    public static CS_LoadSceneOk readBy(ByteBuf buffer){
				    CS_LoadSceneOk ele = new CS_LoadSceneOk();
				    ele.read(buffer);
				    buffer.release();
				    return ele;
			    }
		    
	public int m_RoomId;

	@Override
protected void write(){

		writeInt( m_RoomId);
	}
	@Override
protected void read(){

		 m_RoomId = readInt();
	}
}

public static class Packet_Move extends BinaryMessageStruct{
        
            public byte type()
            {
                return (byte) StructEnum.Packet_Move.value;
            }
			    public static Packet_Move readBy(ByteBuf buffer){
				    Packet_Move ele = new Packet_Move();
				    ele.read(buffer);
				    return ele;
			    }
		    
	public int m_ObjId;
	public int m_x;
	public int m_z;

	@Override
protected void write(){

		writeInt( m_ObjId);
		writeInt( m_x);
		writeInt( m_z);
	}
	@Override
protected void read(){

		 m_ObjId = readInt();
		 m_x = readInt();
		 m_z = readInt();
	}
}

public static class Packet_MovePath extends BinaryMessageStruct{
        
            public byte type()
            {
                return (byte) StructEnum.Packet_MovePath.value;
            }
			    public static Packet_MovePath readBy(ByteBuf buffer){
				    Packet_MovePath ele = new Packet_MovePath();
				    ele.read(buffer);
				    return ele;
			    }
		    
	public int m_ObjId;
	public int m_curx;
	public int m_curz;
	public ArrayList<Integer> m_xlist = new ArrayList<>();
	public ArrayList<Integer> m_zlist = new ArrayList<>();

	@Override
protected void write(){

		writeInt( m_ObjId);
		writeInt( m_curx);
		writeInt( m_curz);
		int _TempSize = 0;
						_TempSize = m_xlist.size();
						writeInt( _TempSize );
						for(int _var : m_xlist)
						{
							writeInt(_var);
						}
				
						_TempSize = m_zlist.size();
						writeInt( _TempSize );
						for(int _var : m_zlist)
						{
							writeInt(_var);
						}
				
	}
	@Override
protected void read(){

		 m_ObjId = readInt();
		 m_curx = readInt();
		 m_curz = readInt();
		int _TempSize = 0;
						_TempSize =  readInt();
						for( int i =0;i< _TempSize;++i)
						{
							int _var = readInt();
							m_xlist.add(_var);
						}
				
						_TempSize =  readInt();
						for( int i =0;i< _TempSize;++i)
						{
							int _var = readInt();
							m_zlist.add(_var);
						}
				
	}
}

public static class Packet_StopMove extends BinaryMessageStruct{
        
            public byte type()
            {
                return (byte) StructEnum.Packet_StopMove.value;
            }
			    public static Packet_StopMove readBy(ByteBuf buffer){
				    Packet_StopMove ele = new Packet_StopMove();
				    ele.read(buffer);
				    return ele;
			    }
		    
	public int m_ObjId;

	@Override
protected void write(){

		writeInt( m_ObjId);
	}
	@Override
protected void read(){

		 m_ObjId = readInt();
	}
}

public static class Packet_SelObj extends BinaryMessageStruct{
        
            public byte type()
            {
                return (byte) StructEnum.Packet_SelObj.value;
            }
			    public static Packet_SelObj readBy(ByteBuf buffer){
				    Packet_SelObj ele = new Packet_SelObj();
				    ele.read(buffer);
				    return ele;
			    }
		    
	public int m_PlayerId;
	public int m_ObjId;

	@Override
protected void write(){

		writeInt( m_PlayerId);
		writeInt( m_ObjId);
	}
	@Override
protected void read(){

		 m_PlayerId = readInt();
		 m_ObjId = readInt();
	}
}

public static class Packet_CreateObj extends BinaryMessageStruct{
        
            public byte type()
            {
                return (byte) StructEnum.Packet_CreateObj.value;
            }
			    public static Packet_CreateObj readBy(ByteBuf buffer){
				    Packet_CreateObj ele = new Packet_CreateObj();
				    ele.read(buffer);
				    return ele;
			    }
		    
	public int m_TemplateId;
	public int m_ObjectType;
	public int m_EntityId;

	@Override
protected void write(){

		writeInt( m_TemplateId);
		writeInt( m_ObjectType);
		writeInt( m_EntityId);
	}
	@Override
protected void read(){

		 m_TemplateId = readInt();
		 m_ObjectType = readInt();
		 m_EntityId = readInt();
	}
}

public static class Packet_UseSkill extends BinaryMessageStruct{
        
            public byte type()
            {
                return (byte) StructEnum.Packet_UseSkill.value;
            }
			    public static Packet_UseSkill readBy(ByteBuf buffer){
				    Packet_UseSkill ele = new Packet_UseSkill();
				    ele.read(buffer);
				    return ele;
			    }
		    
	public int m_ObjId;
	public int m_SkillId;
	public int m_x;
	public int m_y;
	public int m_TargetId;

	@Override
protected void write(){

		writeInt( m_ObjId);
		writeInt( m_SkillId);
		writeInt( m_x);
		writeInt( m_y);
		writeInt( m_TargetId);
	}
	@Override
protected void read(){

		 m_ObjId = readInt();
		 m_SkillId = readInt();
		 m_x = readInt();
		 m_y = readInt();
		 m_TargetId = readInt();
	}
}

    public enum StructEnum
    {
        t_PreparePlayerData(1),
Packet_Move(2),
Packet_MovePath(3),
Packet_StopMove(4),
Packet_SelObj(5),
Packet_CreateObj(6),
Packet_UseSkill(7),
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
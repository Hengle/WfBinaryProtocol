#include "Packet_Battle.pb.h"

namespace rpc
{
	bool t_PreparePlayerData::Serialize( wf::BaseStream& os ) const
	{
		os << m_playerid; if (!os.IsOK()) return false;
		os << m_name; if (!os.IsOK()) return false;
		os << m_jobid; if (!os.IsOK()) return false;
		return os.IsOK();
	}

	bool t_PreparePlayerData::DeSerialize( wf::BaseStream& is )
	{
		is >> m_playerid; if (!is.IsOK()) return false;
		is >> m_name; if (!is.IsOK()) return false;
		is >> m_jobid; if (!is.IsOK()) return false;
		return is.IsOK();
	}


	bool SC_PreBattle::Serialize( wf::BaseStream& os ) const
	{
		os << (uint32)datas.size();
		for( auto& obj : datas )
		{
			obj.Serialize(os); if (!os.IsOK()) return false;
		}
		return os.IsOK();
	}

	bool SC_PreBattle::DeSerialize( wf::BaseStream& is )
	{
		int _TempSize = 0;
		_TempSize =  is.read_uint32();
		for( int i =0;i< _TempSize;++i)
		{
			t_PreparePlayerData obj; obj.DeSerialize(is); if (!is.IsOK()) return false;
			datas.push_back(obj); 
		}
		return is.IsOK();
	}


	bool SC_PrepareOk::Serialize( wf::BaseStream& os ) const
	{
		os << (uint32)datas.size();
		for( auto& obj : datas )
		{
			obj.Serialize(os); if (!os.IsOK()) return false;
		}
		return os.IsOK();
	}

	bool SC_PrepareOk::DeSerialize( wf::BaseStream& is )
	{
		int _TempSize = 0;
		_TempSize =  is.read_uint32();
		for( int i =0;i< _TempSize;++i)
		{
			t_PreparePlayerData obj; obj.DeSerialize(is); if (!is.IsOK()) return false;
			datas.push_back(obj); 
		}
		return is.IsOK();
	}


	bool CS_LoadSceneOk::Serialize( wf::BaseStream& os ) const
	{
		os << m_RoomId; if (!os.IsOK()) return false;
		return os.IsOK();
	}

	bool CS_LoadSceneOk::DeSerialize( wf::BaseStream& is )
	{
		is >> m_RoomId; if (!is.IsOK()) return false;
		return is.IsOK();
	}


	bool Packet_Move::Serialize( wf::BaseStream& os ) const
	{
		os << m_ObjId; if (!os.IsOK()) return false;
		os << m_x; if (!os.IsOK()) return false;
		os << m_z; if (!os.IsOK()) return false;
		return os.IsOK();
	}

	bool Packet_Move::DeSerialize( wf::BaseStream& is )
	{
		is >> m_ObjId; if (!is.IsOK()) return false;
		is >> m_x; if (!is.IsOK()) return false;
		is >> m_z; if (!is.IsOK()) return false;
		return is.IsOK();
	}


	bool Packet_MovePath::Serialize( wf::BaseStream& os ) const
	{
		os << m_ObjId; if (!os.IsOK()) return false;
		os << m_curx; if (!os.IsOK()) return false;
		os << m_curz; if (!os.IsOK()) return false;
		os << (uint32)m_xlist.size();
		for( auto& obj : m_xlist )
		{
			os << obj; if (!os.IsOK()) return false;
		}
		os << (uint32)m_zlist.size();
		for( auto& obj : m_zlist )
		{
			os << obj; if (!os.IsOK()) return false;
		}
		return os.IsOK();
	}

	bool Packet_MovePath::DeSerialize( wf::BaseStream& is )
	{
		is >> m_ObjId; if (!is.IsOK()) return false;
		is >> m_curx; if (!is.IsOK()) return false;
		is >> m_curz; if (!is.IsOK()) return false;
		int _TempSize = 0;
		_TempSize =  is.read_uint32();
		for( int i =0;i< _TempSize;++i)
		{
			int obj; is >> obj; if (!is.IsOK()) return false;
			m_xlist.push_back(obj); 
		}
		_TempSize =  is.read_uint32();
		for( int i =0;i< _TempSize;++i)
		{
			int obj; is >> obj; if (!is.IsOK()) return false;
			m_zlist.push_back(obj); 
		}
		return is.IsOK();
	}


	bool Packet_StopMove::Serialize( wf::BaseStream& os ) const
	{
		os << m_ObjId; if (!os.IsOK()) return false;
		return os.IsOK();
	}

	bool Packet_StopMove::DeSerialize( wf::BaseStream& is )
	{
		is >> m_ObjId; if (!is.IsOK()) return false;
		return is.IsOK();
	}


	bool Packet_SelObj::Serialize( wf::BaseStream& os ) const
	{
		os << m_PlayerId; if (!os.IsOK()) return false;
		os << m_ObjId; if (!os.IsOK()) return false;
		return os.IsOK();
	}

	bool Packet_SelObj::DeSerialize( wf::BaseStream& is )
	{
		is >> m_PlayerId; if (!is.IsOK()) return false;
		is >> m_ObjId; if (!is.IsOK()) return false;
		return is.IsOK();
	}


	bool Packet_CreateObj::Serialize( wf::BaseStream& os ) const
	{
		os << m_TemplateId; if (!os.IsOK()) return false;
		os << m_ObjectType; if (!os.IsOK()) return false;
		os << m_EntityId; if (!os.IsOK()) return false;
		return os.IsOK();
	}

	bool Packet_CreateObj::DeSerialize( wf::BaseStream& is )
	{
		is >> m_TemplateId; if (!is.IsOK()) return false;
		is >> m_ObjectType; if (!is.IsOK()) return false;
		is >> m_EntityId; if (!is.IsOK()) return false;
		return is.IsOK();
	}


	bool Packet_UseSkill::Serialize( wf::BaseStream& os ) const
	{
		os << m_ObjId; if (!os.IsOK()) return false;
		os << m_SkillId; if (!os.IsOK()) return false;
		os << m_x; if (!os.IsOK()) return false;
		os << m_y; if (!os.IsOK()) return false;
		os << m_TargetId; if (!os.IsOK()) return false;
		return os.IsOK();
	}

	bool Packet_UseSkill::DeSerialize( wf::BaseStream& is )
	{
		is >> m_ObjId; if (!is.IsOK()) return false;
		is >> m_SkillId; if (!is.IsOK()) return false;
		is >> m_x; if (!is.IsOK()) return false;
		is >> m_y; if (!is.IsOK()) return false;
		is >> m_TargetId; if (!is.IsOK()) return false;
		return is.IsOK();
	}

}

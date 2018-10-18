#include "Packet_Login.pb.h"

namespace rpc
{
	bool CS_Login::Serialize( wf::BaseStream& os ) const
	{
		os << m_type; if (!os.IsOK()) return false;
		os << m_account; if (!os.IsOK()) return false;
		os << m_pwd; if (!os.IsOK()) return false;
		os << m_equipment; if (!os.IsOK()) return false;
		os << m_machine; if (!os.IsOK()) return false;
		return os.IsOK();
	}

	bool CS_Login::DeSerialize( wf::BaseStream& is )
	{
		is >> m_type; if (!is.IsOK()) return false;
		is >> m_account; if (!is.IsOK()) return false;
		is >> m_pwd; if (!is.IsOK()) return false;
		is >> m_equipment; if (!is.IsOK()) return false;
		is >> m_machine; if (!is.IsOK()) return false;
		return is.IsOK();
	}


	bool SC_LoginRst::Serialize( wf::BaseStream& os ) const
	{
		os << m_rst; if (!os.IsOK()) return false;
		os << m_sessionid; if (!os.IsOK()) return false;
		return os.IsOK();
	}

	bool SC_LoginRst::DeSerialize( wf::BaseStream& is )
	{
		is >> m_rst; if (!is.IsOK()) return false;
		is >> m_sessionid; if (!is.IsOK()) return false;
		return is.IsOK();
	}


	bool CS_Register::Serialize( wf::BaseStream& os ) const
	{
		os << m_account; if (!os.IsOK()) return false;
		os << m_pwd; if (!os.IsOK()) return false;
		return os.IsOK();
	}

	bool CS_Register::DeSerialize( wf::BaseStream& is )
	{
		is >> m_account; if (!is.IsOK()) return false;
		is >> m_pwd; if (!is.IsOK()) return false;
		return is.IsOK();
	}


	bool SC_RegisterRst::Serialize( wf::BaseStream& os ) const
	{
		os << m_rst; if (!os.IsOK()) return false;
		return os.IsOK();
	}

	bool SC_RegisterRst::DeSerialize( wf::BaseStream& is )
	{
		is >> m_rst; if (!is.IsOK()) return false;
		return is.IsOK();
	}


	bool CS_LoginGateWay::Serialize( wf::BaseStream& os ) const
	{
		os << m_sessionid; if (!os.IsOK()) return false;
		return os.IsOK();
	}

	bool CS_LoginGateWay::DeSerialize( wf::BaseStream& is )
	{
		is >> m_sessionid; if (!is.IsOK()) return false;
		return is.IsOK();
	}


	bool SC_LoginGateWayRst::Serialize( wf::BaseStream& os ) const
	{
		os << m_rst; if (!os.IsOK()) return false;
		os << m_playerid; if (!os.IsOK()) return false;
		return os.IsOK();
	}

	bool SC_LoginGateWayRst::DeSerialize( wf::BaseStream& is )
	{
		is >> m_rst; if (!is.IsOK()) return false;
		is >> m_playerid; if (!is.IsOK()) return false;
		return is.IsOK();
	}


	bool SC_PlayerInfo::Serialize( wf::BaseStream& os ) const
	{
		os << m_playerid; if (!os.IsOK()) return false;
		os << m_nickname; if (!os.IsOK()) return false;
		os << m_money; if (!os.IsOK()) return false;
		return os.IsOK();
	}

	bool SC_PlayerInfo::DeSerialize( wf::BaseStream& is )
	{
		is >> m_playerid; if (!is.IsOK()) return false;
		is >> m_nickname; if (!is.IsOK()) return false;
		is >> m_money; if (!is.IsOK()) return false;
		return is.IsOK();
	}

}

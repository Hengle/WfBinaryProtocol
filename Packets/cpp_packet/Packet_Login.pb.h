#pragma once
#include "WfSerializePacket.h"

namespace rpc
{enum emLoginResult{
	emLoginResult_Failed = 1,
	emLoginResult_Timeout = 2,
	emLoginResult_OK = 0,
	emLoginResult_None = 255,
};

enum emPacket_Login{
	EM_CS_Login = 257,
	EM_SC_LoginRst = 258,
	EM_CS_Register = 259,
	EM_SC_RegisterRst = 260,
	EM_CS_LoginGateWay = 261,
	EM_SC_LoginGateWayRst = 262,
	em_SC_PlayerInfo = 263,
};


 class CS_Login;
 class SC_LoginRst;
 class CS_Register;
 class SC_RegisterRst;
 class CS_LoginGateWay;
 class SC_LoginGateWayRst;
 class SC_PlayerInfo;

class CS_Login : public ISerializePacket
{
public:
	int m_type;
	std::string m_account;
	std::string m_pwd;
	std::string m_equipment;
	std::string m_machine;
public:
	bool Serialize( wf::BaseStream& os ) const;
	bool DeSerialize( wf::BaseStream& is );
};

class SC_LoginRst : public ISerializePacket
{
public:
	int m_rst;
	std::string m_sessionid;
public:
	bool Serialize( wf::BaseStream& os ) const;
	bool DeSerialize( wf::BaseStream& is );
};

class CS_Register : public ISerializePacket
{
public:
	std::string m_account;
	std::string m_pwd;
public:
	bool Serialize( wf::BaseStream& os ) const;
	bool DeSerialize( wf::BaseStream& is );
};

class SC_RegisterRst : public ISerializePacket
{
public:
	int m_rst;
public:
	bool Serialize( wf::BaseStream& os ) const;
	bool DeSerialize( wf::BaseStream& is );
};

class CS_LoginGateWay : public ISerializePacket
{
public:
	std::string m_sessionid;
public:
	bool Serialize( wf::BaseStream& os ) const;
	bool DeSerialize( wf::BaseStream& is );
};

class SC_LoginGateWayRst : public ISerializePacket
{
public:
	int m_rst;
	uint32 m_playerid;
public:
	bool Serialize( wf::BaseStream& os ) const;
	bool DeSerialize( wf::BaseStream& is );
};

class SC_PlayerInfo : public ISerializePacket
{
public:
	uint32 m_playerid;
	std::string m_nickname;
	uint64 m_money;
public:
	bool Serialize( wf::BaseStream& os ) const;
	bool DeSerialize( wf::BaseStream& is );
};

}

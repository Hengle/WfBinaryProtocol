#pragma once
#include "WfSerializePacket.h"

namespace rpc
{enum emInputEvent{
	emInputEvent_MovePath = 1,
	emInputEvent_StopMove = 2,
	emInputEvent_SelObj = 3,
	emInputEvent_CreateObj = 4,
	emInputEvent_UseSkill = 5,
	emInputEvent_Move = 0,
};

enum emPacket_Battle{
	em_Battle_Begin = 3840,
	em_SC_PreBattle = 3841,
	em_CS_LoadSceneOk = 3842,
	em_SC_PrepareOk = 3843,
	em_SC_StartBattle = 3844,
	em_CS_InputEvent = 3845,
	em_SC_Fps = 3846,
	em_Battle_End = 4095,
};


 class t_PreparePlayerData;
 class SC_PreBattle;
 class SC_PrepareOk;
 class CS_LoadSceneOk;
 class Packet_Move;
 class Packet_MovePath;
 class Packet_StopMove;
 class Packet_SelObj;
 class Packet_CreateObj;
 class Packet_UseSkill;

class t_PreparePlayerData : public ISerializePacket
{
public:
	uint32 m_playerid;
	std::string m_name;
	uint32 m_jobid;
public:
	bool Serialize( wf::BaseStream& os ) const;
	bool DeSerialize( wf::BaseStream& is );
};

class SC_PreBattle : public ISerializePacket
{
public:
	std::vector<t_PreparePlayerData> datas;
public:
	bool Serialize( wf::BaseStream& os ) const;
	bool DeSerialize( wf::BaseStream& is );
};

class SC_PrepareOk : public ISerializePacket
{
public:
	std::vector<t_PreparePlayerData> datas;
public:
	bool Serialize( wf::BaseStream& os ) const;
	bool DeSerialize( wf::BaseStream& is );
};

class CS_LoadSceneOk : public ISerializePacket
{
public:
	uint32 m_RoomId;
public:
	bool Serialize( wf::BaseStream& os ) const;
	bool DeSerialize( wf::BaseStream& is );
};

class Packet_Move : public ISerializePacket
{
public:
	int m_ObjId;
	int m_x;
	int m_z;
public:
	bool Serialize( wf::BaseStream& os ) const;
	bool DeSerialize( wf::BaseStream& is );
};

class Packet_MovePath : public ISerializePacket
{
public:
	int m_ObjId;
	int m_curx;
	int m_curz;
	std::vector<int> m_xlist;
	std::vector<int> m_zlist;
public:
	bool Serialize( wf::BaseStream& os ) const;
	bool DeSerialize( wf::BaseStream& is );
};

class Packet_StopMove : public ISerializePacket
{
public:
	int m_ObjId;
public:
	bool Serialize( wf::BaseStream& os ) const;
	bool DeSerialize( wf::BaseStream& is );
};

class Packet_SelObj : public ISerializePacket
{
public:
	int m_PlayerId;
	int m_ObjId;
public:
	bool Serialize( wf::BaseStream& os ) const;
	bool DeSerialize( wf::BaseStream& is );
};

class Packet_CreateObj : public ISerializePacket
{
public:
	int m_TemplateId;
	int m_ObjectType;
	int m_EntityId;
public:
	bool Serialize( wf::BaseStream& os ) const;
	bool DeSerialize( wf::BaseStream& is );
};

class Packet_UseSkill : public ISerializePacket
{
public:
	int m_ObjId;
	uint32 m_SkillId;
	int m_x;
	int m_y;
	int m_TargetId;
public:
	bool Serialize( wf::BaseStream& os ) const;
	bool DeSerialize( wf::BaseStream& is );
};

}

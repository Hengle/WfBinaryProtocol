#pragma once
#include "config.h"
#include "lua-5.3.4\lua.hpp"

class PACKET_API ParserOut
{
public:
	struct t_enum 
	{
		string m_name;
		struct t_enumfield {
			string name;
			int32 index;
		};
		map<int32, t_enumfield*> m_enumfield;
		void release();
		bool add_field(const t_enumfield& field);
		void lua_pushdata(lua_State *L);
	};
	struct t_message{
		enum emEessagefieldType {
			emEessagefieldType_general = 0,
			emEessagefieldType_map = 1,
			emEessagefieldType_list = 2			
		};
		struct t_messagefield {
			t_messagefield()
			{
				type = emEessagefieldType_general;
				key_type = TYPE_INT32;
				value_type = TYPE_DEFINE;
				key_type_name = "";
				value_type_name = "";
			}
			emEessagefieldType type;
			Type key_type;
			Type value_type;
			string key_type_name;
			string value_type_name;

			string name;
			uint8 index;
			void lua_pushdata(lua_State *L);
		};
		t_message();
		void release();
		bool add_message(t_message* pMessage);
		bool add_enum(t_enum* pEnum);
		bool add_field(const t_messagefield& field);
		void lua_pushdata(lua_State *L);
		string m_name;
		map<uint8,t_messagefield*> m_messagefield;
		map<uint32, t_message*> m_childMessage;
		unordered_map<string, t_enum*> m_childEnum;
	private:
		uint32 m_messageindex;
	};
public:
	ParserOut();
	bool has_package();
	void append_package(const string& value);
	bool add_message(t_message* pMessage);
	bool add_enum(t_enum* pEnum);
	int tolua(lua_State *L,const char* type,const char* outPath, const char* filename, const char* fight);
	void lua_pushenum(lua_State *L);
	void lua_pushmessage(lua_State *L);
	void lua_pushpacket(lua_State *L);
	void lua_pushimport(lua_State *L);
private:
	uint32 m_messageindex;
	vector<string> m_package;
	unordered_map<string, t_enum*> m_enum;
	map<uint32, t_message*> m_message;
};


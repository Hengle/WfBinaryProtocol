#include "ParserOut.h"
void ParserOut::t_enum::release()
{
	for (auto& t : m_enumfield)
	{
		delete t.second;
	}
	m_enumfield.clear();
	delete this;
}
bool ParserOut::t_enum::add_field(const t_enumfield& field)
{
	auto t = m_enumfield.find(field.index);
	if (t != m_enumfield.end())
	{
		return false;
	}
	t_enumfield* pField = new t_enumfield();
	*pField = field;
	m_enumfield[field.index] = pField;
	return true;
}
void ParserOut::t_enum::lua_pushdata(lua_State *L)
{
	lua_newtable(L);
	for (auto&t : m_enumfield)
	{
		lua_pushinteger(L, t.second->index);
		lua_pushstring(L, t.second->name.c_str());		
		lua_settable(L, -3);
	}
}
void ParserOut::t_message::t_messagefield::lua_pushdata(lua_State *L)
{
	lua_newtable(L);
	lua_pushstring(L, "name");
	lua_pushstring(L, name.c_str());
	lua_settable(L, -3);
	lua_pushstring(L, "type");
	lua_pushinteger(L, type);
	lua_settable(L, -3);
	lua_pushstring(L, "key");
	lua_pushinteger(L, key_type);
	lua_settable(L, -3);
	lua_pushstring(L, "value");
	lua_pushinteger(L, value_type);
	lua_settable(L, -3);
	lua_pushstring(L, "keyname");
	lua_pushstring(L, key_type_name.c_str());
	lua_settable(L, -3);
	lua_pushstring(L, "valuename");
	lua_pushstring(L, value_type_name.c_str());
	lua_settable(L, -3);
	lua_pushstring(L, "index");
	lua_pushinteger(L, index);
	lua_settable(L, -3);
}
ParserOut::t_message::t_message()
{
	m_messageindex = 1;
}
void ParserOut::t_message::release()
{
	for (auto& t : m_childEnum)
	{
		delete t.second;
	}
	m_childEnum.clear();
	for (auto& t : m_childMessage)
	{
		t.second->release();
	}
	m_childMessage.clear();
	delete this;
}
bool ParserOut::t_message::add_message(t_message* pMessage)
{
	if (!pMessage) return false;

	//auto t = m_childMessage.find(pMessage->m_name);
	//if (t != m_childMessage.end())
	//{
	//	return false;
	//}
	m_childMessage[m_messageindex++] = pMessage;
	return true;
}
bool ParserOut::t_message::add_enum(t_enum* pEnum)
{
	if (!pEnum) return false;

	auto t = m_childEnum.find(pEnum->m_name);
	if (t != m_childEnum.end())
	{
		return false;
	}
	m_childEnum[pEnum->m_name] = pEnum;
	return true;
}
bool ParserOut::t_message::add_field(const t_messagefield& field)
{
	auto t = m_messagefield.find(field.index);
	if (t != m_messagefield.end())
	{
		return false;
	}
	t_messagefield* pField = new t_messagefield();
	*pField = field;
	m_messagefield[field.index] = pField;
	return true;
}
void ParserOut::t_message::lua_pushdata(lua_State *L)
{
	lua_newtable(L);
	if (!m_childEnum.empty())
	{
		lua_pushstring(L, "enum");
		lua_newtable(L);
		for (auto&t : m_childEnum)
		{
			lua_pushstring(L, t.first.c_str());
			t.second->lua_pushdata(L);
			lua_settable(L, -3);
		}
		lua_settable(L, -3);
	}
	if (!m_childMessage.empty())
	{
		lua_pushstring(L, "message");
		lua_newtable(L);
		for (auto&t : m_childMessage)
		{
			lua_pushinteger(L, t.first);
			t.second->lua_pushdata(L);
			lua_settable(L, -3);
		}
		lua_settable(L, -3);
	}
	

	lua_pushstring(L, "data");
	lua_newtable(L);
	for (auto&t : m_messagefield)
	{
		lua_pushinteger(L, t.second->index);
		t.second->lua_pushdata(L);
		lua_settable(L, -3);
	}
	lua_settable(L, -3);

	lua_pushstring(L, "name");
	lua_pushstring(L, m_name.c_str());
	lua_settable(L, -3);
}
ParserOut::ParserOut()
{
	m_messageindex = 1;
}
bool ParserOut::has_package()
{
	if (m_package.empty()) {
		return false;
	}
	return true;
}
void ParserOut::append_package(const string& value)
{
	m_package.push_back(value);
}
bool ParserOut::add_message(t_message* pMessage)
{
	if (!pMessage) return false;

	/*auto t = m_message.find(pMessage->m_name);
	if (t != m_message.end())
	{
		return false;
	}*/

	m_message[m_messageindex++] = pMessage;
	return true;
}
bool ParserOut::add_enum(t_enum* pEnum)
{
	if (!pEnum) return false;

	auto t = m_enum.find(pEnum->m_name);
	if (t != m_enum.end())
	{
		return false;
	}
	m_enum[pEnum->m_name] = pEnum;
	return true;
}
int ParserOut::tolua(lua_State *L, const char* type, const char* outPath, const char* filename, const char* fight)
{
	lua_getglobal(L, "main");
	lua_pushstring(L, type);
	lua_pushstring(L, outPath);
	lua_pushstring(L, filename);
	lua_pushstring(L, fight);
	lua_pushenum(L);
	lua_pushmessage(L);
	lua_pushpacket(L);
	return lua_pcall(L, 7, 0,0);
}
void ParserOut::lua_pushenum(lua_State *L)
{
	lua_newtable(L);
	for (auto&t : m_enum)
	{
		lua_pushstring(L, t.first.c_str());
		t.second->lua_pushdata(L);
		lua_settable(L, -3);
	}
}
void ParserOut::lua_pushmessage(lua_State *L)
{
	lua_newtable(L);
	for (auto&t : m_message)
	{
		lua_pushinteger(L, t.first);
		t.second->lua_pushdata(L);
		lua_settable(L, -3);
	}
}
void ParserOut::lua_pushpacket(lua_State *L)
{
	lua_newtable(L);
	
	for (int i = 0;i<m_package.size();i++)
	{
		lua_pushinteger(L, i+1);
		lua_pushstring(L, m_package[i].c_str());
		lua_settable(L, -3);
	}
}
void ParserOut::lua_pushimport(lua_State *L)
{

}
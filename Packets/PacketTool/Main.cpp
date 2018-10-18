#include "config.h"
# include <deque>
# include <stack>
# include <string>
# include <iostream>
#include <fstream>
//#include<Parser.h>
#include "tokenizer.h"
#include "Parser.h"
#include "lua-5.3.4\lua.hpp"
#include <algorithm> 
#include <vector>
using namespace std;

bool ParseArgument(const char* arg,	string* name, string* value) 
{
	if (arg[0] == '-') {
		// Two dashes:  Multi-character name, with '=' separating name and
		//   value.
		const char* equals_pos = strchr(arg, '=');
		if (equals_pos != NULL) {
			*name = string(arg, equals_pos - arg);
			*value = equals_pos + 1;
			return true;
		}
	}
	return false;
}
bool ParseArguments(int argc, char **argv, string* type, string* outpath, string* inputpath, string* fileName, string* fight)
{
	std::vector<string> arguments;
	for (int i = 1; i < argc; ++i) {
		arguments.push_back(argv[i]);
	}
	for (auto&t : arguments)
	{
		string key;
		string value;
		if (!ParseArgument(t.c_str(), &key, &value))
			return false;
		transform(key.begin(), key.end(), key.begin(), ::tolower);

		if (key.compare("-type") == 0 || key.compare("-t") == 0)
		{
			*type = value;
		}
		else if (key.compare("-outpath") == 0 || key.compare("-o") == 0) 
		{
			*outpath = value;
		}
		else if (key.compare("-inputpath") == 0 || key.compare("-i") == 0) 
		{
			*inputpath = value;
		}
		else if (key.compare("-filename") == 0 || key.compare("-f") == 0)
		{
			*fileName = value;
		}
		else if (key.compare("-fight") == 0)
		{
			*fight = value;
		}
	}
	return true;
}

aa::aa()
{}
bb::bb()
{}
int main(int argc, char **argv)
{
	bb test;
	string fileName = "";
	string type = "cs";
	string outpath = "./";
	string inputpath = "./";
	string fight = "false";
	if (!ParseArguments(argc, argv,&type,&outpath,&inputpath,&fileName, &fight))
	{
		printf("arg error.\n");
		return 0;
	}
	
	std::ifstream file(inputpath + fileName+ ".proto", std::ios::_Nocreate);
	if (!file.good()) {
		return false;
	}

	std::string doc;
	std::getline(file, doc, (char)EOF);
	
	// Set up the tokenizer and parser.
	io::ErrorCollector file_error_collector;
	io::Tokenizer tokenizer(doc.c_str(), doc.size(), &file_error_collector);

	ParserOut out;
	// Parse it.
	Parser parser;
	bool rst =parser.Parse(&tokenizer,&out);

	lua_State *L = luaL_newstate();
	luaL_openlibs(L);

	if (luaL_loadfile(L, "./PacketCreater.lua")) {
		printf("error %s\n", lua_tostring(L, -1));
		return -1;
	}
	lua_pcall(L, 0, 0, 0);
	int tmp = out.tolua(L, type.c_str(), outpath.c_str(), fileName.c_str(), fight.c_str());
	if(tmp){
		const char *pErrorMsg = lua_tostring(L, -1);
		printf(pErrorMsg);
	}

	return 0;
}
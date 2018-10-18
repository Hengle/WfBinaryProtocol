#pragma once
#include "config.h"
//#include "platform.h"
//# include <deque>
//# include <stack>
# include <string>
# include <iostream>
#include "Value.h"

class PACKET_API Reader
{
public:
	typedef char Char;
	typedef const char *Location;
	Reader();
	//bool parse(const std::string &document,
	//	Value &root,
	//	bool collectComments = true);
	//bool parse(std::istream &is,
	//	Value &root,
	//	bool collectComments = true);
	bool parse(const char *beginDoc, const char *endDoc,PacketDescriptor &root);

private:
	Location begin_;
	Location end_;
	Location current_;
private:
	enum TokenType
	{
		tokenEndOfStream = 0,
		tokenObjectBegin,
		tokenObjectEnd,
		tokenArrayBegin,
		tokenArrayEnd,
		tokenString,
		tokenNumber,
		tokenTrue,
		tokenFalse,
		tokenNull,
		tokenArraySeparator,
		tokenMemberSeparator,
		tokenComment,
		tokenError,

		TYPE_START,       // Next() has not yet been called.
		TYPE_END,         // End of input reached.  "text" is empty.
	};

	class Token
	{
	public:
		TokenType type_;
		std::string text;
		Location start_;
		Location end_;
	};
private:
	Token m_current;           
	Token m_previous;         
private:
	bool Next();
	void skipSpaces();
	Char getNextChar();
	void readNumber();
	bool readString();
	bool match(Location pattern, int patternLength);
	bool readComment();
	bool readCStyleComment();
	bool readCppStyleComment();
	void skipCommentTokens(Token &token);
private:
	// True if the current token is TYPE_END.
	inline bool AtEnd();
	// True if the next token matches the given text.
	inline bool LookingAt(const char* text);
	// True if the next token is of the given type.
	inline bool LookingAtType(TokenType token_type);
	bool Reader::ParseTopLevelStatement();
};


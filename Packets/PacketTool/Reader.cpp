#include "Reader.h"

bool Reader::parse(const char *beginDoc, const char *endDoc, PacketDescriptor &root)
{
	begin_ = beginDoc;
	end_ = endDoc;
	current_ = begin_;

	Token token;
	skipCommentTokens(token);

	while (!AtEnd()) {
		if (!ParseTopLevelStatement()) {
			Token token;
			skipCommentTokens(token);

			if (LookingAt("}")) {
				printf("Unmatched \"}\".");
				return false;
			}
		}
	}
	return true;
}
bool Reader::Next()
{
	m_previous = m_current;

	skipSpaces();
	token.start_ = current_;
	Char c = getNextChar();
	bool ok = true;
	switch (c)
	{
	case '{':
		token.type_ = tokenObjectBegin;
		break;
	case '}':
		token.type_ = tokenObjectEnd;
		break;
	case '[':
		token.type_ = tokenArrayBegin;
		break;
	case ']':
		token.type_ = tokenArrayEnd;
		break;
	case '"':
		token.type_ = tokenString;
		ok = readString();
		break;
	case '/':
		token.type_ = tokenComment;
		ok = readComment();
		break;
	case '0':
	case '1':
	case '2':
	case '3':
	case '4':
	case '5':
	case '6':
	case '7':
	case '8':
	case '9':
	case '-':
		token.type_ = tokenNumber;
		readNumber();
		break;
	case 't':
		token.type_ = tokenTrue;
		ok = match("rue", 3);
		break;
	case 'f':
		token.type_ = tokenFalse;
		ok = match("alse", 4);
		break;
	case 'n':
		token.type_ = tokenNull;
		ok = match("ull", 3);
		break;
	case ',':
		token.type_ = tokenArraySeparator;
		break;
	case ':':
		token.type_ = tokenMemberSeparator;
		break;
	case 0:
		token.type_ = tokenEndOfStream;
		break;
	default:
		ok = false;
		break;
	}
	if (!ok)
		token.type_ = tokenError;
	token.end_ = current_;
	return true;
}
void Reader::skipSpaces()
{
	while (current_ != end_)
	{
		Char c = *current_;
		if (c == ' ' || c == '\t' || c == '\r' || c == '\n')
			++current_;
		else
			break;
	}
}
Reader::Char Reader::getNextChar()
{
	if (current_ == end_)
		return 0;
	return *current_++;
}
void Reader::readNumber()
{
	while (current_ != end_)
	{
		if (!(*current_ >= '0'  &&  *current_ <= '9') &&
			!in(*current_, '.', 'e', 'E', '+', '-'))
			break;
		++current_;
	}
}

bool Reader::readString()
{
	Char c = 0;
	while (current_ != end_)
	{
		c = getNextChar();
		if (c == '\\')
			getNextChar();
		else if (c == '"')
			break;
	}
	return c == '"';
}
bool Reader::match(Location pattern,int patternLength)
{
	if (end_ - current_ < patternLength)
		return false;
	int index = patternLength;
	while (index--)
		if (current_[index] != pattern[index])
			return false;
	current_ += patternLength;
	return true;
}
bool Reader::readComment()
{
	Location commentBegin = current_ - 1;
	Char c = getNextChar();
	bool successful = false;
	if (c == '*')
		successful = readCStyleComment();
	else if (c == '/')
		successful = readCppStyleComment();
	if (!successful)
		return false;
	return true;
}
bool Reader::readCStyleComment()
{
	while (current_ != end_)
	{
		Char c = getNextChar();
		if (c == '*'  &&  *current_ == '/')
			break;
	}
	return getNextChar() == '/';
}


bool Reader::readCppStyleComment()
{
	while (current_ != end_)
	{
		Char c = getNextChar();
		if (c == '\r' || c == '\n')
			break;
	}
	return true;
}
void Reader::skipCommentTokens(Token &token)
{
	do
	{
		readToken(token);
	} while (token.type_ == tokenComment);
}

inline bool Reader::LookingAt(const char* text) {
	return m_curToken.text == text;
}

inline bool Reader::LookingAtType(TokenType token_type) {
	return m_curToken.type_ == token_type;
}

inline bool Reader::AtEnd() {
	return LookingAtType(TokenType::TYPE_END);
}

bool Reader::ParseTopLevelStatement() 
{
	if (TryConsumeEndOfDeclaration(";", NULL)) {
		// empty statement; ignore
		return true;
	}
	else if (LookingAt("message")) {
		LocationRecorder location(root_location,
			FileDescriptorProto::kMessageTypeFieldNumber, file->message_type_size());
		return ParseMessageDefinition(file->add_message_type(), location, file);
	}
	else if (LookingAt("enum")) {
		LocationRecorder location(root_location,
			FileDescriptorProto::kEnumTypeFieldNumber, file->enum_type_size());
		return ParseEnumDefinition(file->add_enum_type(), location, file);
	}
	else if (LookingAt("service")) {
		LocationRecorder location(root_location,
			FileDescriptorProto::kServiceFieldNumber, file->service_size());
		return ParseServiceDefinition(file->add_service(), location, file);
	}
	else if (LookingAt("extend")) {
		LocationRecorder location(root_location,
			FileDescriptorProto::kExtensionFieldNumber);
		return ParseExtend(file->mutable_extension(),
			file->mutable_message_type(),
			root_location,
			FileDescriptorProto::kMessageTypeFieldNumber,
			location, file);
	}
	else if (LookingAt("import")) {
		return ParseImport(file->mutable_dependency(),
			file->mutable_public_dependency(),
			file->mutable_weak_dependency(),
			root_location, file);
	}
	else if (LookingAt("package")) {
		return ParsePackage(file, root_location, file);
	}
	else if (LookingAt("option")) {
		LocationRecorder location(root_location,
			FileDescriptorProto::kOptionsFieldNumber);
		return ParseOption(file->mutable_options(), location, file,
			OPTION_STATEMENT);
	}
	else {
		AddError("Expected top-level statement (e.g. \"message\").");
		return false;
	}
}
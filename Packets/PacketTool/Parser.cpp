#include "Parser.h"
#include <float.h>
#include <limits>
#include "common.h"
#include <unordered_map>

namespace {
	TypeNameMap MakeTypeNameTable() {
		TypeNameMap result;
		result["int8"] = TYPE_INT8;
		result["uint8"] = TYPE_UINT8;
		result["int16"] = TYPE_INT16;
		result["uint16"] = TYPE_UINT16;
		result["int32"] = TYPE_INT32;
		result["uint32"] = TYPE_UINT32;
		result["int64"] = TYPE_INT64;
		result["uint64"] = TYPE_UINT64;

		result["float"] = TYPE_FLOAT;
		result["double"] = TYPE_DOUBLE;
		result["bool"] = TYPE_BOOL;
		result["string"] = TYPE_STRING;
		return result;
	}
	NameTypeMap MakeNameTypeTable() {
		NameTypeMap result;
		result[TYPE_INT8] = "int8";
		result[TYPE_UINT8] = "uint8";
		result[TYPE_INT16] = "int16";
		result[TYPE_UINT16] = "uint16";
		result[TYPE_INT32] = "int32";
		result[TYPE_UINT32] = "uint32";
		result[TYPE_INT64] = "int64";
		result[TYPE_UINT64] = "uint64";

		result[TYPE_FLOAT] = "float";
		result[TYPE_DOUBLE] = "double";
		result[TYPE_BOOL] = "bool";
		result[TYPE_STRING] = "string";
		return result;
	}
const TypeNameMap kTypeNames = MakeTypeNameTable();
const NameTypeMap kNameTypes = MakeNameTypeTable();

// Camel-case the field name and append "Entry" for generated map entry name.
// e.g. map<KeyType, ValueType> foo_map => FooMapEntry
string MapEntryName(const string& field_name) {
  string result;
  static const char kSuffix[] = "Entry";
  result.reserve(field_name.size() + sizeof(kSuffix));
  bool cap_next = true;
  for (int i = 0; i < field_name.size(); ++i) {
    if (field_name[i] == '_') {
      cap_next = true;
    } else if (cap_next) {
      // Note: Do not use ctype.h due to locales.
      if ('a' <= field_name[i] && field_name[i] <= 'z') {
        result.push_back(field_name[i] - 'a' + 'A');
      } else {
        result.push_back(field_name[i]);
      }
      cap_next = false;
    } else {
      result.push_back(field_name[i]);
    }
  }
  result.append(kSuffix);
  return result;
}

}  // anonymous namespace

// Makes code slightly more readable.  The meaning of "DO(foo)" is
// "Execute foo and fail if it fails.", where failure is indicated by
// returning false.


// ===================================================================

Parser::Parser() 
{
}

Parser::~Parser() {
}

// ===================================================================

inline bool Parser::LookingAt(const char* text) {
  return input_->current().text == text;
}

inline bool Parser::LookingAtType(io::Tokenizer::TokenType token_type) {
  return input_->current().type == token_type;
}

inline bool Parser::AtEnd() {
  return LookingAtType(io::Tokenizer::TYPE_END);
}

bool Parser::TryConsume(const char* text) {
  if (LookingAt(text)) {
    input_->Next();
    return true;
  } else {
    return false;
  }
}

bool Parser::Consume(const char* text, const char* error) {
  if (TryConsume(text)) {
    return true;
  } else {
    AddError(error);
    return false;
  }
}

bool Parser::Consume(const char* text) {
  if (TryConsume(text)) {
    return true;
  } else {
    AddError("Expected \"" + string(text) + "\".");
    return false;
  }
}

bool Parser::ConsumeIdentifier(string* output, const char* error) {
  if (LookingAtType(io::Tokenizer::TYPE_IDENTIFIER)) {
    *output = input_->current().text;
    input_->Next();
    return true;
  } else {
    AddError(error);
    return false;
  }
}

bool Parser::ConsumeInteger(int* output, const char* error) {
  if (LookingAtType(io::Tokenizer::TYPE_INTEGER)) {
    uint64 value = 0;
    if (!io::Tokenizer::ParseInteger(input_->current().text,
                                     kint32max, &value)) {
      AddError("Integer out of range.");
      // We still return true because we did, in fact, parse an integer.
    }
    *output = value;
    input_->Next();
    return true;
  } else {
    AddError(error);
    return false;
  }
}

bool Parser::ConsumeSignedInteger(int* output, const char* error) {
  bool is_negative = false;
  uint64 max_value = kint32max;
  if (TryConsume("-")) {
    is_negative = true;
    max_value += 1;
  }
  uint64 value = 0;
  DO(ConsumeInteger64(max_value, &value, error));
  if (is_negative) value *= -1;
  *output = value;
  return true;
}

bool Parser::ConsumeInteger64(uint64 max_value, uint64* output,
                              const char* error) {
  if (LookingAtType(io::Tokenizer::TYPE_INTEGER)) {
    if (!io::Tokenizer::ParseInteger(input_->current().text, max_value,
                                     output)) {
      AddError("Integer out of range.");
      // We still return true because we did, in fact, parse an integer.
      *output = 0;
    }
    input_->Next();
    return true;
  } else {
    AddError(error);
    return false;
  }
}

bool Parser::ConsumeNumber(double* output, const char* error) {
  if (LookingAtType(io::Tokenizer::TYPE_FLOAT)) {
    *output = io::Tokenizer::ParseFloat(input_->current().text);
    input_->Next();
    return true;
  } else if (LookingAtType(io::Tokenizer::TYPE_INTEGER)) {
    // Also accept integers.
    uint64 value = 0;
    if (!io::Tokenizer::ParseInteger(input_->current().text,
                                     kuint64max, &value)) {
      AddError("Integer out of range.");
      // We still return true because we did, in fact, parse a number.
    }
    *output = value;
    input_->Next();
    return true;
  } else if (LookingAt("inf")) {
    *output = std::numeric_limits<double>::infinity();
    input_->Next();
    return true;
  } else if (LookingAt("nan")) {
    *output = std::numeric_limits<double>::quiet_NaN();
    input_->Next();
    return true;
  } else {
    AddError(error);
    return false;
  }
}

bool Parser::ConsumeString(string* output, const char* error) {
  if (LookingAtType(io::Tokenizer::TYPE_STRING)) {
    io::Tokenizer::ParseString(input_->current().text, output);
    input_->Next();
    // Allow C++ like concatenation of adjacent string tokens.
    while (LookingAtType(io::Tokenizer::TYPE_STRING)) {
      io::Tokenizer::ParseStringAppend(input_->current().text, output);
      input_->Next();
    }
    return true;
  } else {
    AddError(error);
    return false;
  }
}

bool Parser::TryConsumeEndOfDeclaration(
    const char* text) {
  if (LookingAt(text)) {
    string leading, trailing;
    std::vector<string> detached;
    input_->NextWithComments(&trailing, &detached, &leading);

    // Save the leading comments for next time, and recall the leading comments
    // from last time.
    leading.swap(upcoming_doc_comments_);

    if (strcmp(text, "}") == 0) {
      // If the current location is null and we are finishing the current scope,
      // drop pending upcoming detached comments.
      upcoming_detached_comments_.swap(detached);
    } else {
      // Otherwise, append the new detached comments to the existing upcoming
      // detached comments.
      upcoming_detached_comments_.insert(upcoming_detached_comments_.end(),
                                         detached.begin(), detached.end());
    }

    return true;
  } else {
    return false;
  }
}

bool Parser::ConsumeEndOfDeclaration(
    const char* text) {
  if (TryConsumeEndOfDeclaration(text)) {
    return true;
  } else {
    AddError("Expected \"" + string(text) + "\".");
    return false;
  }
}

// -------------------------------------------------------------------

void Parser::AddError(int line, int column, const string& error) {
  if (error_collector_ != NULL) {
    error_collector_->AddError(line, column, error);
  }
  printf("error line(%d) colum(%d):%s", line, column, error.c_str());
  had_errors_ = true;
}

void Parser::AddError(const string& error) {
  AddError(input_->current().line, input_->current().column, error);
}


// -------------------------------------------------------------------

void Parser::SkipStatement() {
  while (true) {
    if (AtEnd()) {
      return;
    } else if (LookingAtType(io::Tokenizer::TYPE_SYMBOL)) {
      if (TryConsumeEndOfDeclaration(";")) {
        return;
      } else if (TryConsume("{")) {
        SkipRestOfBlock();
        return;
      } else if (LookingAt("}")) {
        return;
      }
    }
    input_->Next();
  }
}

void Parser::SkipRestOfBlock() {
  while (true) {
    if (AtEnd()) {
      return;
    } else if (LookingAtType(io::Tokenizer::TYPE_SYMBOL)) {
      if (TryConsumeEndOfDeclaration("}")) {
        return;
      } else if (TryConsume("{")) {
        SkipRestOfBlock();
      }
    }
    input_->Next();
  }
}


bool Parser::Parse(io::Tokenizer* input, ParserOut* out) {
  input_ = input;
  data_ = out;

  if (LookingAtType(io::Tokenizer::TYPE_START)) {
    // Advance to first token.
    input_->NextWithComments(NULL, &upcoming_detached_comments_,
                             &upcoming_doc_comments_);
  }

  {
    if (LookingAt("syntax")) {
      if (!ParseSyntaxIdentifier()) {
        return false;
      }
    } 
    // Repeatedly parse statements until we reach the end of the file.
    while (!AtEnd()) {
      if (!ParseTopLevelStatement() ){
        // This statement failed to parse.  Skip it, but keep looping to parse
        // other statements.
        SkipStatement();

        if (LookingAt("}")) {
          AddError("Unmatched \"}\".");
          input_->NextWithComments(NULL, &upcoming_detached_comments_,
                                   &upcoming_doc_comments_);
        }
      }
    }
  }
  return true;
}

bool Parser::ParseSyntaxIdentifier() {
  DO(Consume(
      "syntax",
      "File must begin with a syntax statement, e.g. 'syntax = \"proto2\";'."));
  DO(Consume("="));
  io::Tokenizer::Token syntax_token = input_->current();
  string syntax;
  DO(ConsumeString(&syntax, "Expected syntax identifier."));
  DO(ConsumeEndOfDeclaration(";"));

  if (syntax != "proto2" && syntax != "proto3" ) {
    AddError(syntax_token.line, syntax_token.column,
      "Unrecognized syntax identifier \"" + syntax + "\".  This parser "
      "only recognizes \"proto2\" and \"proto3\".");
    return false;
  }

  return true;
}

bool Parser::ParseTopLevelStatement() {
  if (TryConsumeEndOfDeclaration(";")) {
    // empty statement; ignore
    return true;
  } else if (LookingAt("message")) {
	  ParserOut::t_message* pMessage = new ParserOut::t_message();
	  if (ParseMessageDefinition(pMessage))
	  {
		  if (!data_->add_message(pMessage)) {
			  AddError("message \"" + pMessage->m_name + "\" repeated");
			  pMessage->release();
			  return false;
		  }
		  return true;
	  }
	  AddError("error  message parse");
	  return false;
  } else if (LookingAt("enum")) {
	  ParserOut::t_enum* pEnum = new ParserOut::t_enum();
	  if (ParseEnumDefinition(pEnum))
	  {
		  if (!data_->add_enum(pEnum)) {
			  AddError("enum \"" + pEnum->m_name + "\" repeated");
			  pEnum->release();
			  return false;
		  }
		  return true;
	  }
	  AddError("error  enum parse");
	  return false;
  } else if (LookingAt("import")) {
    return ParseImport();
  } else if (LookingAt("package")) {
	  return ParsePackage();
  }
  else {
    AddError("Expected top-level statement (e.g. \"message\").");
    return false;
  }
}

// -------------------------------------------------------------------
// Messages

bool Parser::ParseMessageDefinition(ParserOut::t_message* pMessage) {
  DO(Consume("message"));
  DO(ConsumeIdentifier(&pMessage->m_name, "Expected message name.")); 
  DO(ConsumeEndOfDeclaration("{"));
  while (!TryConsumeEndOfDeclaration("}")) {
	  if (AtEnd()) {
		  AddError("Reached end of input in message definition (missing '}').");
		  return false;
	  }

	  if (!ParseMessageStatement(pMessage)) {
		  AddError("ParseMessageStatement");
		  return false;
	  }
  }
  return true;
}

bool Parser::ParseEnumDefinition(ParserOut::t_enum* pEnum)
{
	DO(Consume("enum"));
	DO(ConsumeIdentifier(&pEnum->m_name, "Expected enum name."));
	DO(ConsumeEndOfDeclaration("{"));
	while (!TryConsumeEndOfDeclaration("}")) {
		if (AtEnd()) {
			AddError("Reached end of input in message definition (missing '}').");
			return false;
		}

		if (!ParseEnumStatement(pEnum)) {
			AddError("ParseEnumStatement");
			return false;
		}
	}
	return true;
}
bool Parser::ParsePackage()
{
	if (data_->has_package()) {
		AddError("Multiple package definitions.");
		return false;
	}

	DO(Consume("package"));
	{
		while (true) {
			string identifier;
			DO(ConsumeIdentifier(&identifier, "Expected identifier."));
			data_->append_package(identifier);
			if (!TryConsume(".")) break;
		}
		DO(ConsumeEndOfDeclaration(";"));
	}

	return true;
}
bool Parser::ParseImport()
{
	return true;
}
bool Parser::ParseMessageStatement(ParserOut::t_message* pMessage) {
	
	if (TryConsumeEndOfDeclaration(";")) {
		// empty statement; ignore
		return true;
	}
	else if (LookingAt("message")) {
		ParserOut::t_message* pChildMessage = new ParserOut::t_message();
		if (ParseMessageDefinition(pChildMessage))
		{
			if (!pMessage->add_message(pChildMessage)) {
				AddError("message \"" + pChildMessage->m_name + "\" repeated");
				pChildMessage->release();
				return false;
			}
			return true;
		}
		AddError("error  message parse");
		return false;
	}
	else if (LookingAt("enum")) {
		ParserOut::t_enum* pChildEnum = new ParserOut::t_enum();
		if (ParseEnumDefinition(pChildEnum))
		{
			if (!pMessage->add_enum(pChildEnum)) {
				AddError("enum \"" + pChildEnum->m_name + "\" repeated");
				pChildEnum->release();
				return false;
			}
			return true;
		}
		AddError("error  enum parse");
		return false;
	}
	return ParseMessageField(pMessage);
}
bool Parser::ParseEnumStatement(ParserOut::t_enum* pEnum)
{
	if (TryConsumeEndOfDeclaration(";")) {
		// empty statement; ignore
		return true;
	}
	return ParseEnumField(pEnum);
}
bool Parser::ParseMessageField(ParserOut::t_message* pMessage)
{
	ParserOut::t_message::t_messagefield field;
	if (TryConsume("map")) {
		if (!LookingAt("<")) {
			AddError("map error");
			return false;
		}
		DO(Consume("<"));
		DO(ParseType(&field.key_type, &field.key_type_name));
		DO(Consume(","));
		DO(ParseType(&field.value_type, &field.value_type_name));
		DO(Consume(">"));
		field.type = ParserOut::t_message::emEessagefieldType_map;
	}else if(TryConsume("list")){
		DO(Consume("<"));
		DO(ParseType(&field.key_type, &field.key_type_name));
		DO(Consume(">"));
		field.type = ParserOut::t_message::emEessagefieldType_list;
	}else {
		DO(ParseType(&field.key_type, &field.key_type_name));
		field.type = ParserOut::t_message::emEessagefieldType_general;
	}

	DO(ConsumeIdentifier(&field.name, "Expected field name."));
	DO(Consume("=", "Missing field number."));

	// Parse field number.
	int number;
	DO(ConsumeInteger(&number, "Expected field number."));
	field.index = number;

	DO(ConsumeEndOfDeclaration(";"));
	
	if (!pMessage->add_field(field))
	{
		return false;
	}
	return true;
}
bool Parser::ParseEnumField(ParserOut::t_enum* pEnum)
{
	ParserOut::t_enum::t_enumfield field;

	DO(ConsumeIdentifier(&field.name, "Expected field name."));
	DO(Consume("=", "Missing field number."));

	// Parse field number.
	int number;
	DO(ConsumeInteger(&number, "Expected field number."));
	field.index = number;

	DO(ConsumeEndOfDeclaration(";"));

	if (!pEnum->add_field(field))
	{
		return false;
	}
	return true;
}
bool Parser::ParseType(Type* type,string* type_name) {
	TypeNameMap::const_iterator iter = kTypeNames.find(input_->current().text);
	if (iter != kTypeNames.end()) {
		*type = iter->second;
		*type_name = input_->current().text;
		input_->Next();
	}
	else {
		*type = TYPE_DEFINE;
		*type_name = input_->current().text;
		input_->Next();
	}
	return true;
}


#pragma once
#include "config.h"
#include <map>
#include <string>
#include <utility>
#include "tokenizer.h"
#include "FileDescriptorProto.h"
#include "ParserOut.h"
// Defined in this file.
class Parser;
class SourceLocationTable;




class Parser {
 public:
  Parser();
  ~Parser();

  // Parse the entire input and construct a FileDescriptorProto representing
  // it.  Returns true if no errors occurred, false otherwise.
  bool Parse(io::Tokenizer* input, ParserOut* out);

  // Optional features:

  // DEPRECATED:  New code should use the SourceCodeInfo embedded in the
  //   FileDescriptorProto.
  //
  // Requests that locations of certain definitions be recorded to the given
  // SourceLocationTable while parsing.  This can be used to look up exact line
  // and column numbers for errors reported by DescriptorPool during validation.
  // Set to NULL (the default) to discard source location information.
  void RecordSourceLocationsTo(SourceLocationTable* location_table) {

  }

  // Requests that errors be recorded to the given ErrorCollector while
  // parsing.  Set to NULL (the default) to discard error messages.
  void RecordErrorsTo(io::ErrorCollector* error_collector) {
    error_collector_ = error_collector;
  }

  // If set true, input files will be required to begin with a syntax
  // identifier.  Otherwise, files may omit this.  If a syntax identifier
  // is provided, it must be 'syntax = "proto2";' and must appear at the
  // top of this file regardless of whether or not it was required.
  void SetRequireSyntaxIdentifier(bool value) {

  }

  // Call SetStopAfterSyntaxIdentifier(true) to tell the parser to stop
  // parsing as soon as it has seen the syntax identifier, or lack thereof.
  // This is useful for quickly identifying the syntax of the file without
  // parsing the whole thing.  If this is enabled, no error will be recorded
  // if the syntax identifier is something other than "proto2" (since
  // presumably the caller intends to deal with that), but other kinds of
  // errors (e.g. parse errors) will still be reported.  When this is enabled,
  // you may pass a NULL FileDescriptorProto to Parse().
  void SetStopAfterSyntaxIdentifier(bool value) {

  }

 private:

  // =================================================================
  // Error recovery helpers

  // Consume the rest of the current statement.  This consumes tokens
  // until it sees one of:
  //   ';'  Consumes the token and returns.
  //   '{'  Consumes the brace then calls SkipRestOfBlock().
  //   '}'  Returns without consuming.
  //   EOF  Returns (can't consume).
  // The Parser often calls SkipStatement() after encountering a syntax
  // error.  This allows it to go on parsing the following lines, allowing
  // it to report more than just one error in the file.
  void SkipStatement();

  // Consume the rest of the current block, including nested blocks,
  // ending after the closing '}' is encountered and consumed, or at EOF.
  void SkipRestOfBlock();

  // -----------------------------------------------------------------
  // Single-token consuming helpers
  //
  // These make parsing code more readable.

  // True if the current token is TYPE_END.
  inline bool AtEnd();

  // True if the next token matches the given text.
  inline bool LookingAt(const char* text);
  // True if the next token is of the given type.
  inline bool LookingAtType(io::Tokenizer::TokenType token_type);

  // If the next token exactly matches the text given, consume it and return
  // true.  Otherwise, return false without logging an error.
  bool TryConsume(const char* text);

  // These attempt to read some kind of token from the input.  If successful,
  // they return true.  Otherwise they return false and add the given error
  // to the error list.

  // Consume a token with the exact text given.
  bool Consume(const char* text, const char* error);
  // Same as above, but automatically generates the error "Expected \"text\".",
  // where "text" is the expected token text.
  bool Consume(const char* text);
  // Consume a token of type IDENTIFIER and store its text in "output".
  bool ConsumeIdentifier(string* output, const char* error);
  // Consume an integer and store its value in "output".
  bool ConsumeInteger(int* output, const char* error);
  // Consume a signed integer and store its value in "output".
  bool ConsumeSignedInteger(int* output, const char* error);
  // Consume a 64-bit integer and store its value in "output".  If the value
  // is greater than max_value, an error will be reported.
  bool ConsumeInteger64(uint64 max_value, uint64* output, const char* error);
  // Consume a number and store its value in "output".  This will accept
  // tokens of either INTEGER or FLOAT type.
  bool ConsumeNumber(double* output, const char* error);
  // Consume a string literal and store its (unescaped) value in "output".
  bool ConsumeString(string* output, const char* error);

  // Consume a token representing the end of the statement.  Comments between
  // this token and the next will be harvested for documentation.  The given
  // LocationRecorder should refer to the declaration that was just parsed;
  // it will be populated with these comments.
  //
  // TODO(kenton):  The LocationRecorder is const because historically locations
  //   have been passed around by const reference, for no particularly good
  //   reason.  We should probably go through and change them all to mutable
  //   pointer to make this more intuitive.
  bool TryConsumeEndOfDeclaration(
      const char* text);

  bool ConsumeEndOfDeclaration(
      const char* text);

  // -----------------------------------------------------------------
  // Error logging helpers

  // Invokes error_collector_->AddError(), if error_collector_ is not NULL.
  void AddError(int line, int column, const string& error);

  // Invokes error_collector_->AddError() with the line and column number
  // of the current token.
  void AddError(const string& error);

  // =================================================================
  // Parsers for various language constructs

  // Parses the "syntax = \"proto2\";" line at the top of the file.  Returns
  // false if it failed to parse or if the syntax identifier was not
  // recognized.
  bool ParseSyntaxIdentifier();

  // These methods parse various individual bits of code.  They return
  // false if they completely fail to parse the construct.  In this case,
  // it is probably necessary to skip the rest of the statement to recover.
  // However, if these methods return true, it does NOT mean that there
  // were no errors; only that there were no *syntax* errors.  For instance,
  // if a service method is defined using proper syntax but uses a primitive
  // type as its input or output, ParseMethodField() still returns true
  // and only reports the error by calling AddError().  In practice, this
  // makes logic much simpler for the caller.

  // Parse a top-level message, enum, service, etc.
  bool ParseTopLevelStatement();

  // Parse various language high-level language construrcts.
  bool ParseMessageDefinition(ParserOut::t_message* pMessage);
  bool ParseEnumDefinition(ParserOut::t_enum* pEnum);
  bool ParsePackage();
  bool ParseImport();
 
  bool ParseMessageStatement(ParserOut::t_message* pMessage);
  bool ParseEnumStatement(ParserOut::t_enum* pEnum);

  bool ParseMessageField(ParserOut::t_message* pMessage);
  bool ParseEnumField(ParserOut::t_enum* pEnum);
  bool ParseType(Type* type, string* type_name);
  bool had_errors_;
  io::Tokenizer* input_;
  ParserOut* data_;
  io::ErrorCollector* error_collector_;

  string upcoming_doc_comments_;
  std::vector<string> upcoming_detached_comments_;
 };





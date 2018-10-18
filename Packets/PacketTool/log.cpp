#include "log.h"

LogMessage::LogMessage(LogLevel level, const char* filename, int line)
	: level_(level), filename_(filename), line_(line) {}
LogMessage::~LogMessage() {}

void LogMessage::Finish() {
	
}
LogMessage& LogMessage::operator<<(const string& value) {
	message_ += value;
	return *this;
}

LogMessage& LogMessage::operator<<(const char* value) {
	message_ += value;
	return *this;
}

void LogFinisher::operator=(LogMessage& other) {
	other.Finish();
}

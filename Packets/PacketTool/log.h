#pragma once
#include "config.h"

enum LogLevel {
  LOGLEVEL_INFO,     // Informational.  This is never actually used by
                     // libprotobuf.
  LOGLEVEL_WARNING,  // Warns about issues that, although not technically a
                     // problem now, could cause problems in the future.  For
                     // example, a // warning will be printed when parsing a
                     // message that is near the message size limit.
  LOGLEVEL_ERROR,    // An error occurred which should never happen during
                     // normal use.
  LOGLEVEL_FATAL,    // An error occurred from which the library cannot
                     // recover.  This usually indicates a programming error
                     // in the code which calls the library, especially when
                     // compiled in debug mode.

#ifdef NDEBUG
  LOGLEVEL_DFATAL = LOGLEVEL_ERROR
#else
  LOGLEVEL_DFATAL = LOGLEVEL_FATAL
#endif
};

class PACKET_API LogMessage {
public:
	LogMessage(LogLevel level, const char* filename, int line);
	~LogMessage();

	LogMessage& operator<<(const std::string& value);
	LogMessage& operator<<(const char* value);
	

private:
	friend class LogFinisher;
	void Finish();

	LogLevel level_;
	const char* filename_;
	int line_;
	std::string message_;
};
// Used to make the entire "LOG(BLAH) << etc." expression have a void return
// type and print a newline after each message.
class PACKET_API LogFinisher {
public:
	void operator=(LogMessage& other);
};

template<typename T>
bool IsOk(T status) { return status.ok(); }
template<>
inline bool IsOk(bool status) { return status; }


#define GOOGLE_LOG(LEVEL)	LogFinisher() = LogMessage(LOGLEVEL_##LEVEL, __FILE__, __LINE__)
#define GOOGLE_LOG_IF(LEVEL, CONDITION) !(CONDITION) ? (void)0 : GOOGLE_LOG(LEVEL)

#define GOOGLE_CHECK(EXPRESSION) GOOGLE_LOG_IF(FATAL, !(EXPRESSION)) << "CHECK failed: " #EXPRESSION ": "
#define GOOGLE_CHECK_OK(A) GOOGLE_CHECK(::google::protobuf::internal::IsOk(A))
#define GOOGLE_CHECK_EQ(A, B) GOOGLE_CHECK((A) == (B))
#define GOOGLE_CHECK_NE(A, B) GOOGLE_CHECK((A) != (B))
#define GOOGLE_CHECK_LT(A, B) GOOGLE_CHECK((A) <  (B))
#define GOOGLE_CHECK_LE(A, B) GOOGLE_CHECK((A) <= (B))
#define GOOGLE_CHECK_GT(A, B) GOOGLE_CHECK((A) >  (B))
#define GOOGLE_CHECK_GE(A, B) GOOGLE_CHECK((A) >= (B))

namespace internal {
	template<typename T>
	T* CheckNotNull(const char* /* file */, int /* line */,
		const char* name, T* val) {
		if (val == NULL) {
			GOOGLE_LOG(FATAL) << name;
		}
		return val;
	}
}  // namespace internal
#define GOOGLE_CHECK_NOTNULL(A) \
  ::google::protobuf::internal::CheckNotNull(\
      __FILE__, __LINE__, "'" #A "' must not be NULL", (A))

#ifdef NDEBUG

#define GOOGLE_DLOG(LEVEL) GOOGLE_LOG_IF(LEVEL, false)

#define GOOGLE_DCHECK(EXPRESSION) while(false) GOOGLE_CHECK(EXPRESSION)
#define GOOGLE_DCHECK_OK(E) GOOGLE_DCHECK(::google::protobuf::internal::IsOk(E))
#define GOOGLE_DCHECK_EQ(A, B) GOOGLE_DCHECK((A) == (B))
#define GOOGLE_DCHECK_NE(A, B) GOOGLE_DCHECK((A) != (B))
#define GOOGLE_DCHECK_LT(A, B) GOOGLE_DCHECK((A) <  (B))
#define GOOGLE_DCHECK_LE(A, B) GOOGLE_DCHECK((A) <= (B))
#define GOOGLE_DCHECK_GT(A, B) GOOGLE_DCHECK((A) >  (B))
#define GOOGLE_DCHECK_GE(A, B) GOOGLE_DCHECK((A) >= (B))

#else  // NDEBUG

#define GOOGLE_DLOG GOOGLE_LOG

#define GOOGLE_DCHECK    GOOGLE_CHECK
#define GOOGLE_DCHECK_OK GOOGLE_CHECK_OK
#define GOOGLE_DCHECK_EQ GOOGLE_CHECK_EQ
#define GOOGLE_DCHECK_NE GOOGLE_CHECK_NE
#define GOOGLE_DCHECK_LT GOOGLE_CHECK_LT
#define GOOGLE_DCHECK_LE GOOGLE_CHECK_LE
#define GOOGLE_DCHECK_GT GOOGLE_CHECK_GT
#define GOOGLE_DCHECK_GE GOOGLE_CHECK_GE

#endif  // !NDEBUG
#pragma once

#if defined(PACKET_DLL_BUILD)
#  define PACKET_API __declspec(dllexport)
# elif defined(Packet_DLL)
#  define PACKET_API __declspec(dllimport)
# else
#  define PACKET_API
# endif

typedef unsigned int	uint32;
typedef unsigned short	uint16;
typedef unsigned char	uint8;
typedef int				int32;
typedef short			int16;
typedef char			int8;
typedef unsigned short	ushort;
typedef unsigned int	uint;
typedef unsigned char	uchar;

#ifdef _MSC_VER
typedef unsigned __int64 uint64;
typedef __int64 int64;
#else
typedef unsigned long long uint64;
typedef long long int64;
#endif

#define DO(STATEMENT) if (STATEMENT) {} else return false


#include <string>
#include <vector>
#include <unordered_map>
#include <map>
using namespace std;


extern string StringPrintf(const char* format, ...);


#ifdef _MSC_VER
#define GOOGLE_LONGLONG(x) x##I64
#define GOOGLE_ULONGLONG(x) x##UI64
#define GOOGLE_LL_FORMAT "I64"  // As in printf("%I64d", ...)
#else
// By long long, we actually mean int64.
#define GOOGLE_LONGLONG(x) x##LL
#define GOOGLE_ULONGLONG(x) x##ULL
// Used to format real long long integers.
#define GOOGLE_LL_FORMAT "ll"  // As in "%lld". Note that "q" is poor form also.
#endif
static const int32 kint32max = 0x7FFFFFFF;
static const int32 kint32min = -kint32max - 1;
static const int64 kint64max = GOOGLE_LONGLONG(0x7FFFFFFFFFFFFFFF);
static const int64 kint64min = -kint64max - 1;
static const uint32 kuint32max = 0xFFFFFFFFu;
static const uint64 kuint64max = GOOGLE_ULONGLONG(0xFFFFFFFFFFFFFFFF);

enum Type {
	TYPE_DEFINE = 0,
	TYPE_INT8 = 1,
	TYPE_UINT8 = 2,
	TYPE_INT16 = 3,
	TYPE_UINT16 = 4,
	TYPE_INT32 = 5,
	TYPE_UINT32 = 6,
	TYPE_INT64 = 7,
	TYPE_UINT64 = 8,
	TYPE_FLOAT = 9,
	TYPE_DOUBLE = 10,
	TYPE_BOOL = 11,
	TYPE_STRING = 12
};
typedef unordered_map<string, Type> TypeNameMap;
typedef unordered_map<Type, string> NameTypeMap;

class bb;
class aa
{
public:
	aa();
	bb* m_bb;
};
class bb
{
public:
	bb();
	int ttt;
};
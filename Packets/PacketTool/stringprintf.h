#pragma once
#include "config.h"
#include <stdarg.h>
#include <string>
#include <vector>

// Return a C++ string
PACKET_API extern string StringPrintf(const char* format, ...);

// Store result into a supplied string and return it
PACKET_API extern const string& SStringPrintf(string* dst, const char* format, ...);

// Append result to a supplied string
PACKET_API extern void StringAppendF(string* dst, const char* format, ...);

// Lower-level routine that takes a va_list and appends to a specified
// string.  All other routines are just convenience wrappers around it.
PACKET_API extern void StringAppendV(string* dst, const char* format, va_list ap);





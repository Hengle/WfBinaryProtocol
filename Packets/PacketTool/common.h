#pragma once
#include "config.h"
//#include "platform.h"
//# include <deque>
//# include <stack>
# include <string>
# include <iostream>
#include "log.h"
#include "stringprintf.h"

PACKET_API uint32 ghtonl(uint32 x);

inline string CEscape(const string& src) {
	return src;
}

namespace io {

	// A locale-independent version of the standard strtod(), which always
	// uses a dot as the decimal separator.
	double NoLocaleStrtod(const char* str, char** endptr);

	// Casts a double value to a float value. If the value is outside of the
	// representable range of float, it will be converted to positive or negative
	// infinity.
	float SafeDoubleToFloat(double value);

}  // namespace io
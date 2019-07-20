// C Bindings for Go language service.
// By: Christian Gunderman

// This file is automatically picked up by the "C" Go package during compilation
// and linked into the shared library. We can use it to do some of the more complex
// interop between C# and Go that "C" Go doesn't support, such as calling into
// function pointers.
#ifndef BINDINGS__H__
#define BINDINGS__H__

#include "../../../bin/Go.Interop/net472/golib.h"

GoInt foo();
#endif

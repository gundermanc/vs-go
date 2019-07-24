// C Bindings for Go language service.
// By: Christian Gunderman

// This file is automatically picked up by the "C" Go package during compilation
// and linked into the shared library. We can use it to do some of the more complex
// interop between C# and Go that "C" Go doesn't support, such as calling into
// function pointers.

#include <stdint.h>

#ifndef BINDINGS__H__
#define BINDINGS__H__

// TODO: support release.
//#include "../../../bin/Go.Interop/Debug/net472/golib.h"

typedef struct tagSnapshot
{
    int32_t (*ReadChar)(uint8_t*, int32_t, int32_t);
    int32_t length;
} Snapshot;

// Go can't execute C function pointers, so we have this monstrosity.
int32_t Read(Snapshot snapshot, uint8_t buffer[], int32_t offset, int32_t count);

// .. and this one too
typedef void (*ProvideStringCallback)(uint8_t buffer[], int32_t count);

// .. and this one too.
void InvokeStringCallback(ProvideStringCallback callback, uint8_t buffer[], int32_t count);


#endif

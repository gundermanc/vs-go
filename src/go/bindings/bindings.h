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
    int (*ReadChar)(uint8_t buffer[], int offset, int count);
    int length;
} Snapshot;

// Go can't execute C function pointers, so we have this monstrosity.
int Read(Snapshot snapshot, uint8_t buffer[], int offset, int count);

// .. and this one too
typedef void (*WorkspaceUpdateCallback)(uint8_t buffer[], int count);

// .. and this one too.
void InvokeWorkspaceUpdateCallback(WorkspaceUpdateCallback callback, uint8_t buffer[], int count);

#endif

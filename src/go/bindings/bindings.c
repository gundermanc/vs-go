#include "bindings.h"

int Read(Snapshot snapshot, uint8_t buffer[], int offset, int count)
{
    return snapshot.ReadChar(buffer, offset, count);
}

void InvokeStringCallback(ProvideStringCallback callback, uint8_t buffer[], int count)
{
    callback(buffer, count);
}

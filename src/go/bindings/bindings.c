#include "bindings.h"

int32_t Read(Snapshot snapshot, uint8_t buffer[], int32_t offset, int32_t count)
{
    return snapshot.ReadChar(buffer, offset, count);
}

void InvokeStringCallback(ProvideStringCallback callback, uint8_t buffer[], int32_t count)
{
    callback(buffer, count);
}

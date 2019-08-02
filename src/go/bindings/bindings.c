#include "bindings.h"

int32_t Read(Snapshot snapshot, uint8_t buffer[], int32_t offset, int32_t count)
{
    return snapshot.ReadChar(buffer, offset, count);
}

void InvokeStringCallback(ProvideStringCallback callback, uint8_t buffer[], int32_t count)
{
    callback(buffer, count);
}

void InvokeTokenCallback(ProvideTokenCallback callback, int32_t pos, int32_t end, int32_t type)
{
    callback(pos, end, type);
}

void InvokeWorkspaceUpdatedCallback(WorkspaceUpdatedCallback callback, uint8_t buffer[], int32_t count, void* versionId)
{
    callback(buffer, count, versionId);
}
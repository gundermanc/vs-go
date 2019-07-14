namespace Go.Editor
{
    using System;

    interface ILogger
    {
        LogMessage();
        LogError();
        LogException();
        ComputeExceptionOutputMessage();
        LogString();
        EnsureInitialized();
    }
}

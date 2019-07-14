namespace Go.Editor
{
    using System;

    interface ILogger
    {
        void LogMessage();
        void LogError();
        void LogException();
        string ComputeExceptionOutputMessage();
        void LogString();
        void EnsureInitialized();
    }
}

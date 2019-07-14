namespace Go.Editor
{
    using System;

    public interface ILogger
    {
        void LogMessage();
        void LogError();
        void LogException();
        string ComputeExceptionOutputMessage();
        void LogString();
        void EnsureInitialized();
    }
}

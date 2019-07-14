namespace Go.Editor
{
    using System;
    using System.ComponentModel.Composition;
    using System.Text;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;

    interface ILogger
    {
        void LogMessage(string message)
        {
            this.EnsureInitialized();
            this.LogString(string.Format(Strings.MessageTextFormat, message), activate: true);
        }

        void LogError(string message)
        {
            this.EnsureInitialized();
            this.LogString(string.Format(Strings.ErrorTextFormat, message), activate: true);
        }

        void LogException(Exception ex, string message = null)
        {
            var exceptionOutputMessage = this.ComputeExceptionOutputMessage(ex);
            if (message != null)
            {
                this.LogError(message);
                this.LogString(exceptionOutputMessage);
            }

            else
            {
                this.LogError(exceptionOutputMessage);
            }
        }

        string ComputeExceptionOutputMessage(Exception ex)
        {
            if (ex.InnerException == null)
            {
                return string.Format(Strings.ExceptionFormat, ex.GetType().FullName, ex.Message);
            }

            StringBuilder buffer = new StringBuilder();

            buffer.Append(string.Format(Strings.ExceptionFormat, ex.GetType().FullName, ex.Message));

            for (var i = ex.InnerException; i != null; i = i.InnerException)
            {
                buffer.Append(string.Format(Strings.InnerExceptionFormat, ex.GetType().FullName, ex.Message));
            }

            return buffer.ToString();
        }

        void LogString(string output, bool activate = false)
        {
            this.EnsureInitialized();

            ThreadHelper.ThrowIfNotOnUIThread();

            ErrorHandler.ThrowOnFailure(this.outputPane.OutputString(output));

            ErrorHandler.ThrowOnFailure(this.outputPane.OutputString(Endline));

            if (activate)
            {
                ErrorHandler.ThrowOnFailure(this.outputPane.Activate());
            }
        }

        void EnsureInitialized()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (this.outputPane == null)
            {
                var outputWindowService = this.serviceProvider.GetService(typeof(SVsOutputWindow)) as IVsOutputWindow;
                Guid mutablePaneGuid = ContextoolPaneGuid;

                ErrorHandler.ThrowOnFailure(
                    outputWindowService.CreatePane(
                        ref mutablePaneGuid,
                        Strings.OutputPaneName,
                        fInitVisible: 0,
                        fClearWithSolution: 0));

                ErrorHandler.ThrowOnFailure(outputWindowService.GetPane(ref mutablePaneGuid, out this.outputPane));
            }
        }
    }
}

namespace Go.Editor.Intellisense.QuickInfo
{
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;

    internal sealed class AsyncQuickInfoSource : IAsyncQuickInfoSource
    {
        private readonly ITextBuffer textBuffer;

        public AsyncQuickInfoSource(ITextBuffer textBuffer)
        {
            this.textBuffer = textBuffer;
        }

        public void Dispose()
        {
        }

        public Task<QuickInfoItem> GetQuickInfoItemAsync(IAsyncQuickInfoSession session, CancellationToken cancellationToken)
        {
            var snapshot = this.textBuffer.CurrentSnapshot;
            var triggerPoint = session.GetTriggerPoint(this.textBuffer);

            var triggerPosition = triggerPoint.GetPoint(snapshot).Position;
            var span = this.textBuffer.CurrentSnapshot.CreateTrackingSpan(triggerPosition, 1, SpanTrackingMode.EdgeInclusive);

            // TODO: running tools on events is going to be a common need. Abstract to a base class.
            // TODO: eliminate '.exe' for VS Mac compat.
            var snapshotText = snapshot.GetText();

            // Get actual file name.
            // TODO: this isn't working yet.
            var gogetdocStartInfo = new ProcessStartInfo("gogetdoc.exe", $@"-pos C:\repos\vs-go\test-fodder\hello-world.go:#{triggerPosition}")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            gogetdocStartInfo.Environment.Add("GOPATH", @"C:\repos\vs-go\test-fodder");

            var process = Process.Start(gogetdocStartInfo);
            process.StandardInput.AutoFlush = true;
            process.StandardInput.Write(snapshotText);
            process.StandardInput.Close();

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();

            // TODO: async wrapper for all of these calls to other processes.
            return Task.FromResult(new QuickInfoItem(span, output + error));
        }
    }
}

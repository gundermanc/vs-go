namespace Go.Editor.Intellisense.QuickInfo
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;

    internal sealed class AsyncQuickInfoSource : IAsyncQuickInfoSource
    {
        private readonly ITextDocumentFactoryService textDocumentFactoryService;
        private readonly ITextBuffer textBuffer;

        public AsyncQuickInfoSource(ITextDocumentFactoryService textDocumentFactoryService, ITextBuffer textBuffer)
        {
            this.textDocumentFactoryService = textDocumentFactoryService
                ?? throw new ArgumentNullException(nameof(textDocumentFactoryService));
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

            // Get actual file path.
            if (!this.textDocumentFactoryService.TryGetTextDocument(this.textBuffer, out var textDocument))
            {
                return Task.FromResult(default(QuickInfoItem));
            }
            var documentPath = textDocument.FilePath;

            // TODO: there's an unfortunate constraint of 'gogetdoc' that it is unable to fetch
            // docs for files outside of your GOPATH. Investigate this further and see if we can
            // set a temp path to get stuff working for loose files.
            var gogetdocStartInfo = new ProcessStartInfo("gogetdoc.exe", $"-pos \"{documentPath}:#{triggerPosition}\"")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var process = Process.Start(gogetdocStartInfo);

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();

            // TODO: async wrapper for all of these calls to other processes.
            return Task.FromResult(new QuickInfoItem(span, output + error));
        }
    }
}

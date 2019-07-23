namespace Go.Editor.Intellisense.QuickInfo
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;

    internal sealed class QuickInfoItemSource : IAsyncQuickInfoSource
    {
        private readonly GoWorkspace workspace;
        private readonly ITextBuffer textBuffer;

        public QuickInfoItemSource(GoWorkspace workspace, ITextBuffer textBuffer)
        {
            this.workspace = workspace
                ?? throw new ArgumentNullException(nameof(workspace));
            this.textBuffer = textBuffer
                ?? throw new ArgumentNullException(nameof(textBuffer));
        }

        public void Dispose()
        {
        }

        public Task<QuickInfoItem> GetQuickInfoItemAsync(IAsyncQuickInfoSession session, CancellationToken cancellationToken)
        {
            var triggerSnapshotPoint = session.GetTriggerPoint(this.textBuffer.CurrentSnapshot);
            if (triggerSnapshotPoint.HasValue)
            {

            }
            return null;
        }
    }
}

namespace Go.Editor.Intellisense.QuickInfo
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Go.Editor.Common;
    using Go.Interop.Workspace;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;

    internal sealed class QuickInfoSource : IAsyncQuickInfoSource
    {
        private readonly WorkspaceDocument<ITextBuffer> document;

        public QuickInfoSource(WorkspaceDocument<ITextBuffer> document)
        {
            this.document = document ?? throw new ArgumentNullException(nameof(document));
        }

        public void Dispose() => this.document.Dispose();

        public Task<QuickInfoItem> GetQuickInfoItemAsync(IAsyncQuickInfoSession session, CancellationToken cancellationToken)
        {
            var snapshot = this.document.CurrentSnapshot.ToTextSnapshot();
            var triggerPoint = session.GetTriggerPoint(snapshot);
            if (triggerPoint.HasValue)
            {
                var quickInfoText = this.document.GetQuickInfo(triggerPoint.Value.Position);
                if (!string.IsNullOrWhiteSpace(quickInfoText))
                {
                    // TODO: bounds check.
                    var applicableSpan = snapshot.CreateTrackingSpan(new Span(triggerPoint.Value, 1), SpanTrackingMode.EdgeInclusive);
                    return Task.FromResult(new QuickInfoItem(applicableSpan, quickInfoText));
                }
            }

            return Task.FromResult<QuickInfoItem>(null);
        }
    }
}

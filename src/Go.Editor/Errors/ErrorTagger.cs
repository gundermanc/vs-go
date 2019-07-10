namespace Go.Editor.Errors
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Threading.Tasks;
    using Go.CodeAnalysis.Workspace;
    using Go.Editor.Common;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Adornments;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Threading;

    internal class ErrorTagger : ITagger<IErrorTag>, IDisposable
    {
        private readonly JoinableTaskContext joinableTaskContext;
        private readonly WorkspaceDocument<ITextBuffer> document;
        private ImmutableArray<ITagSpan<IErrorTag>> currentTags = ImmutableArray<ITagSpan<IErrorTag>>.Empty;

        public ErrorTagger(JoinableTaskContext joinableTaskContext, WorkspaceDocument<ITextBuffer> document)
        {
            this.joinableTaskContext = joinableTaskContext ?? throw new ArgumentNullException(nameof(joinableTaskContext));
            this.document = document ?? throw new ArgumentNullException(nameof(document));

            this.document.SnapshotUpdated += this.OnDocumentSnapshotUpdated;
        }

        private async void OnDocumentSnapshotUpdated(object sender, EventArgs e)
        {
            var errorsBuilder = ImmutableArray.CreateBuilder<ITagSpan<IErrorTag>>();

            var documentSnapshot = this.document.CurrentSnapshot;

            foreach (var error in documentSnapshot.Errors)
            {
                // TODO: support other error types.
                errorsBuilder.Add(new TagSpan<IErrorTag>(error.Extent.ToSnapshotSpan(), new ErrorTag(PredefinedErrorTypeNames.CompilerError, error.Message)));
            }

            this.currentTags = errorsBuilder.ToImmutable();

            // TODO: make this incremental.
            await this.RaiseTagsChangedOnUIThreadAsync(documentSnapshot.RootNode.Extent.ToSnapshotSpan());
        }

        private async Task RaiseTagsChangedOnUIThreadAsync(SnapshotSpan snapshotSpan)
        {
            await this.joinableTaskContext.Factory.SwitchToMainThreadAsync();

            this.TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(snapshotSpan));
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<IErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            // TODO: make this incremental.
            return this.currentTags;
        }

        public void Dispose()
        {
            this.document.Dispose();
            this.document.SnapshotUpdated -= this.OnDocumentSnapshotUpdated;
        }
    }
}

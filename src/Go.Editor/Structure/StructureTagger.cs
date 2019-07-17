namespace Go.Editor.Errors
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Threading.Tasks;
    using Go.CodeAnalysis.Editor;
    using Go.CodeAnalysis.Parser;
    using Go.CodeAnalysis.Workspace;
    using Go.Editor.Common;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Threading;

    // TODO: pretty similar to the error tagger. Combine the two into a common base.
    internal class StructureTagger : ITagger<IStructureTag>, IDisposable
    {
        private readonly JoinableTaskContext joinableTaskContext;
        private readonly WorkspaceDocument<ITextBuffer> document;
        private ImmutableArray<ITagSpan<IStructureTag>> currentTags = ImmutableArray<ITagSpan<IStructureTag>>.Empty;

        public StructureTagger(JoinableTaskContext joinableTaskContext, WorkspaceDocument<ITextBuffer> document)
        {
            this.joinableTaskContext = joinableTaskContext ?? throw new ArgumentNullException(nameof(joinableTaskContext));
            this.document = document ?? throw new ArgumentNullException(nameof(document));

            this.document.SnapshotUpdated += this.OnDocumentSnapshotUpdated;
        }

        private async void OnDocumentSnapshotUpdated(object sender, EventArgs e)
        {
            var outliningBuilder = ImmutableArray.CreateBuilder<ITagSpan<IStructureTag>>();

            var documentSnapshot = this.document.CurrentSnapshot;

            // TODO: generalize for non-functions.
            foreach (FunctionDeclarationNode block in documentSnapshot.GetFunctions())
            {
                // TODO: include args in header span.
                // TODO: explicitly specify guideline span + point.
                // TODO: outlining preview + colorization.
                var headerSpan = Span.FromBounds(block.Extent.Start, block.FunctionNameExtent.End);
                var outliningSpan = Span.FromBounds(block.FunctionNameExtent.End, block.Extent.End);
                outliningBuilder.Add(
                    new TagSpan<IStructureTag>(block.Extent.ToSnapshotSpan(),
                    new StructureTag(block.Extent.Snapshot.ToTextSnapshot(), outliningSpan, headerSpan, isCollapsible: true)));
            }

            this.currentTags = outliningBuilder.ToImmutable();

            // TODO: make this incremental.
            await this.RaiseTagsChangedOnUIThreadAsync(documentSnapshot.RootNode.Extent.ToSnapshotSpan());
        }

        private async Task RaiseTagsChangedOnUIThreadAsync(SnapshotSpan snapshotSpan)
        {
            await this.joinableTaskContext.Factory.SwitchToMainThreadAsync();

            this.TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(snapshotSpan));
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<IStructureTag>> GetTags(NormalizedSnapshotSpanCollection spans)
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

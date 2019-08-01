namespace Go.Editor.Tagging
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Go.Editor.Common;
    using Go.Interop.Workspace;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;

    internal sealed class Tagger<TTagType> : ITagger<TTagType>, IDisposable where TTagType : ITag
    {
        private readonly TaggerProviderBase<TTagType> taggerProvider;
        private readonly WorkspaceDocument<ITextBuffer> document;

        private ImmutableArray<ITagSpan<TTagType>> tags = ImmutableArray<ITagSpan<TTagType>>.Empty;

        public Tagger(TaggerProviderBase<TTagType> taggerProvider, WorkspaceDocument<ITextBuffer> document)
        {
            this.taggerProvider = taggerProvider
                ?? throw new ArgumentNullException(nameof(taggerProvider));
            this.document = document
                ?? throw new ArgumentNullException(nameof(document));

            this.document.DocumentUpdated += this.OnDocumentUpdated;
        }

        private void OnDocumentUpdated(object sender, EventArgs e)
        {
            var textSnapshot = this.document.CurrentSnapshot.ToTextSnapshot();
            if (this.taggerProvider.TryGetNewTags(
                this.document,
                this.document.CurrentSnapshot.ToTextSnapshot(),
                out var tags))
            {
                this.tags = tags;
                this.TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(textSnapshot, 0, textSnapshot.Length)));
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<TTagType>> GetTags(NormalizedSnapshotSpanCollection spans) => this.tags;

        public void Dispose() => this.document.Dispose();
    }
}

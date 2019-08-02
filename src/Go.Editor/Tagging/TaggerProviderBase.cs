namespace Go.Editor.Tagging
{
    using System.Collections.Immutable;
    using System.ComponentModel.Composition;
    using Go.Interop.Workspace;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;

    internal abstract class TaggerProviderBase<TTagType> : ITaggerProvider where TTagType : ITag
    {
        [Import]
        protected GoWorkspace Workspace { get; private set; }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            var document = this.Workspace.GetOrCreateDocument(buffer);

            return new Tagger<TTagType>(this, document) as ITagger<T>;
        }

        internal abstract bool TryGetNewTags(
            WorkspaceDocument<ITextBuffer> document,
            ITextSnapshot textSnapshot,
            out ImmutableArray<ITagSpan<TTagType>> tags);
    }
}

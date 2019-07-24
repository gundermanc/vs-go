namespace Go.Editor.Classification
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Go.Editor.Common;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Tagging;

    internal sealed class ClassificationTagger : ITagger<IClassificationTag>
    {
        private readonly GoWorkspace workspace;
        private readonly IClassificationTypeRegistryService classificationTypeRegistry;
        private readonly ITextBuffer textBuffer;
        private ImmutableArray<ITagSpan<IClassificationTag>> classificationTags = ImmutableArray<ITagSpan<IClassificationTag>>.Empty;

        public ClassificationTagger(
            GoWorkspace workspace,
            IClassificationTypeRegistryService classificationTypeRegistry,
            ITextBuffer textBuffer)
        {
            this.workspace = workspace;
            this.classificationTypeRegistry = classificationTypeRegistry;
            this.textBuffer = textBuffer;

            workspace.WorkspaceFileUpdated += this.OnWorkspaceFileUpdated;
        }

        private void OnWorkspaceFileUpdated(object sender, string e)
        {
            var snapshot = this.textBuffer.CurrentSnapshot;

            // TODO: only tags for this file.
            var tokensBuilder = ImmutableArray.CreateBuilder<ITagSpan<IClassificationTag>>();
            foreach (var token in this.workspace.GetFileTokens())
            {
                // TODO: per token.
                var classificationType = this.classificationTypeRegistry.GetClassificationType(token.Type.ToClassificationTypeName());
                var tag = new ClassificationTag(classificationType);

                // TODO: ensure correct snapshot.
                tokensBuilder.Add(
                    new TagSpan<IClassificationTag>(new SnapshotSpan(
                        snapshot,
                        Span.FromBounds(Math.Max(0, token.Pos - 1), token.End)),
                        tag));
            }

            this.classificationTags = tokensBuilder.ToImmutable();
            this.TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(snapshot, 0, snapshot.Length)));
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<IClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return this.classificationTags;
        }
    }
}

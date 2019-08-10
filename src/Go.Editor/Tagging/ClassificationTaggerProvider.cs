namespace Go.Editor.Tagging
{
    using System;
    using System.Collections.Immutable;
    using System.ComponentModel.Composition;
    using Go.Editor.Common;
    using Go.Interop.Workspace;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IClassificationTag))]
    [ContentType(GoContentType.Name)]
    internal sealed class ClassificationTaggerProvider : TaggerProviderBase<IClassificationTag>
    {
        [Import]
        private IClassificationTypeRegistryService classificationTypeRegistry = null;

        internal override bool TryGetNewTags(
            WorkspaceDocument<ITextBuffer> document,
            ITextSnapshot textSnapshot,
            out ImmutableArray<ITagSpan<IClassificationTag>> tags)
        {
            var tokensBuilder = ImmutableArray.CreateBuilder<ITagSpan<IClassificationTag>>();

            foreach (var token in document.GetFileTokens())
            {
                // TODO: per token.
                var classificationType = this.classificationTypeRegistry.GetClassificationType(token.Type.ToClassificationTypeName());
                var tag = new ClassificationTag(classificationType);

                tokensBuilder.Add(
                    new TagSpan<IClassificationTag>(new SnapshotSpan(
                        textSnapshot,
                        Span.FromBounds(Math.Max(0, token.Pos - 1), Math.Min(token.End, textSnapshot.Length))),
                        tag));
            }

            tags = tokensBuilder.ToImmutable();
            return true;
        }
    }
}

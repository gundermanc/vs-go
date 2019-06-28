namespace Go.Editor.Classification
{
    using System;
    using System.Collections.Generic;
    using Go.CodeAnalysis;
    using Go.Editor.Common;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Tagging;

    internal sealed class ClassificationTagger : ITagger<IClassificationTag>
    {
        private readonly IClassificationTypeRegistryService classificationRegistryService;
        private ITextBuffer textBuffer;

        public ClassificationTagger(
            IClassificationTypeRegistryService classificationRegistryService,
            ITextBuffer textBuffer)
        {
            this.classificationRegistryService = classificationRegistryService
                ?? throw new ArgumentNullException(nameof(classificationRegistryService));
            this.textBuffer = textBuffer;
        }

#pragma warning disable 0067
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
#pragma warning restore 0067

        public IEnumerable<ITagSpan<IClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            // TODO: make a full colorizer backed by a parse tree.

            // For now, just slap something together with the lexer.
            var lexer = Lexer.Create(this.textBuffer.CurrentSnapshot.ToSnapshot());
            while (lexer.TryGetNextLexeme(out var lexeme))
            {
                yield return new TagSpan<IClassificationTag>(
                    lexeme.Segment.ToSnapshotSpan(),
                    new ClassificationTag(this.classificationRegistryService.GetClassificationType(lexeme.Type.ToClassificationTypeName())));
            }
        }
    }
}

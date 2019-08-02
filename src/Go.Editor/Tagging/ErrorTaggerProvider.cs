namespace Go.Editor.Tagging
{
    using System.Collections.Immutable;
    using System.ComponentModel.Composition;
    using Go.Interop.Workspace;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Adornments;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IErrorTag))]
    [ContentType(GoContentType.Name)]
    internal sealed class ErrorTaggerProvider : TaggerProviderBase<IErrorTag>
    {
        internal override bool TryGetNewTags(
            WorkspaceDocument<ITextBuffer> document,
            ITextSnapshot textSnapshot,
            out ImmutableArray<ITagSpan<IErrorTag>> tags)
        {
            // TODO: get error spans so we can correctly place the error squiggles.
            var errorTagsBuilder = ImmutableArray.CreateBuilder<ITagSpan<IErrorTag>>();
            foreach (var error in document.GetErrors())
            {
                var lineSegments = error.Split(':');
                if (lineSegments.Length == 3 &&
                    int.TryParse(lineSegments[0], out var lineNumber) &&
                    int.TryParse(lineSegments[1], out var column) &&
                    lineNumber >= 0 &&
                    lineNumber <= textSnapshot.LineCount)
                {
                    var errorLineStart = textSnapshot.GetLineFromLineNumber(lineNumber - 1).Start;
                    var errorSpanStart = errorLineStart.Position + column;

                    // TODO: this won't draw squiggles on the last char in the file.
                    if (errorSpanStart + 1 < textSnapshot.Length)
                    {
                        // TODO: we can probably pass the span back from Go instead of reading it from the error text.
                        // TODO: highlight the whole token.
                        errorTagsBuilder.Add(
                            new TagSpan<IErrorTag>(
                                new SnapshotSpan(textSnapshot, new Span(errorSpanStart, 1)),
                                new ErrorTag(PredefinedErrorTypeNames.SyntaxError, lineSegments[2])));
                    }

                }
            }

            tags = errorTagsBuilder.ToImmutable();
            return true;
        }
    }
}

namespace Go.Editor.Errors
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Go.Editor.Common;
    using Go.Interop;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Adornments;
    using Microsoft.VisualStudio.Text.Tagging;

    internal sealed class ErrorTagger : ITagger<IErrorTag>
    {
        private readonly GoWorkspace workspace;
        private readonly ITextBuffer buffer;
        private ImmutableArray<ITagSpan<IErrorTag>> errorTags = ImmutableArray<ITagSpan<IErrorTag>>.Empty;

        public ErrorTagger(GoWorkspace workspace, ITextBuffer buffer)
        {
            this.workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
            this.buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));

            this.buffer.Changed += this.OnTextBufferChanged;
            this.workspace.WorkspaceFileUpdated += this.OnWorkspaceFileUpdated;
        }

        private void OnWorkspaceFileUpdated(object sender, string fileName)
        {
            var snapshot = this.buffer.CurrentSnapshot;

            // TODO: only errors for this file.
            // TODO: get error spans so we can correctly place the error squiggles.
            // TODO: check which snapshot was used to generate this set of errors.
            var errorTagsBuilder = ImmutableArray.CreateBuilder<ITagSpan<IErrorTag>>();
            foreach (var error in this.workspace.GetErrors())
            {
                var lineSegments = error.Split(':');
                if (lineSegments.Length == 3 &&
                    int.TryParse(lineSegments[0], out var lineNumber) &&
                    int.TryParse(lineSegments[1], out var column) &&
                    lineNumber >= 0 &&
                    lineNumber <= snapshot.LineCount)
                {
                    var errorLineStart = snapshot.GetLineFromLineNumber(lineNumber - 1).Start;
                    var errorSpanStart = errorLineStart.Position + column;

                    // TODO: this won't draw squiggles on the last char in the file.
                    if (errorSpanStart + 1 < snapshot.Length)
                    {
                        // TODO: we can probably pass the span back from Go instead of reading it from the error text.
                        // TODO: highlight the whole token.
                        errorTagsBuilder.Add(
                            new TagSpan<IErrorTag>(
                                new SnapshotSpan(snapshot, new Span(errorSpanStart, 1)),
                                new ErrorTag(PredefinedErrorTypeNames.CompilerError, lineSegments[2])));
                    }

                }
            }

            this.errorTags = errorTagsBuilder.ToImmutable();

            this.TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(new SnapshotPoint(snapshot, 0), snapshot.Length)));
        }

        private void OnTextBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            // Ensure we're parsing again.
            this.workspace.QueueFileParse(
                "Foo.go", // TODO: file name.
                GoSnapshot.FromSnapshot(this.buffer.CurrentSnapshot.ToSnapshot()));
        }

#pragma warning disable 0067
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
#pragma warning restore 0067

        public IEnumerable<ITagSpan<IErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans) => this.errorTags;
    }
}

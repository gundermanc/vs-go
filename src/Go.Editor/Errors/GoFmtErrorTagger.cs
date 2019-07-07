namespace Go.Editor.Errors
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Adornments;
    using Microsoft.VisualStudio.Text.Operations;
    using Microsoft.VisualStudio.Text.Tagging;

    /// <summary>
    /// Hacky stand-in for real error handling that uses Gofmt.exe for basic syntax errors.
    /// TODO: delete this once our parser is mature enough to give similar errors. For now we have both.
    /// </summary>
    internal class GoFmtErrorTagger : ITagger<IErrorTag>
    {
        private readonly ITextStructureNavigator textStructureNavigator;
        private readonly ITextBuffer textBuffer;
        private ImmutableArray<ITagSpan<IErrorTag>> errorTagSnapshot = ImmutableArray<ITagSpan<IErrorTag>>.Empty;

        public GoFmtErrorTagger(ITextBuffer textBuffer, ITextStructureNavigator textStructureNavigator)
        {
            this.textBuffer = textBuffer ?? throw new ArgumentNullException(nameof(textBuffer));

            // TODO: do we need to unsubscribe?
            if (textBuffer is ITextBuffer2 textBuffer2)
            {
                textBuffer2.ChangedOnBackground += this.OnTextBufferChangedOnBackground;
            }

            this.textStructureNavigator = textStructureNavigator ?? throw new ArgumentNullException(nameof(textStructureNavigator));

            // Create first errors snapshot.
            this.OnTextBufferChangedOnBackground(null, null);
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<IErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            // TODO: back this by a parse tree built on top of our lexer and use gofmt
            // only for periodic 'deep' error checks for things we may have missed.

            // TODO: we need a workspace-wide awareness of symbols so we can check for semantic errors
            // as well as syntax errors provided by gofmt.

            // For now though.. use pipe in stuff from Gofmt to light up some syntax error squiggles.
            // This should be asynchronous and off teh UI thread, but the focus for now is to get as
            // many demoable features to spark interest ;)

            // TODO: search for and return only the tags that intersect with 'spans'.
            return this.errorTagSnapshot;
        }

        private void OnTextBufferChangedOnBackground(object sender, TextContentChangedEventArgs e)
        {
            // TODO: we're unsynchronized on a background thread. Add synchronization + cancellation of running tasks.

            // TODO: running tools on events is going to be a common need. Abstract to a base class.
            // TODO: avoid stringifying the entire buffer.
            // TODO: eliminate '.exe' for VS Mac compat.
            var snapshot = e?.After ?? this.textBuffer.CurrentSnapshot;
            var snapshotText = snapshot.GetText();
            var gofmtStartInfo = new ProcessStartInfo("gofmt.exe", "-e")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var process = Process.Start(gofmtStartInfo);
            process.StandardInput.AutoFlush = true;
            process.StandardInput.Write(snapshotText);
            process.StandardInput.Close();

            // TODO: read input asynchronously in a streaming fashion.

            var errorBuilder = ImmutableArray.CreateBuilder<ITagSpan<IErrorTag>>();
            do
            {
                string line = process.StandardError.ReadLine();

                // Check for errors.
                // Format is <standard input>:LINE:COLUMN: MESSAGE
                if (line?.StartsWith("<standard input>") ?? false)
                {
                    var lineSegments = line.Split(':');
                    if (lineSegments.Length == 4 &&
                        int.TryParse(lineSegments[1], out var lineNumber) &&
                        int.TryParse(lineSegments[2], out var column) &&
                        lineNumber >= 0 &&
                        lineNumber < snapshot.LineCount)
                    {

                        var errorLineStart = snapshot.GetLineFromLineNumber(lineNumber - 1).Start;
                        var errorSpanStart = errorLineStart.Position + column;
                        SnapshotSpan errorTokenSpan = default;
                        if (errorSpanStart >= snapshot.Length)
                        {
                            if (snapshot.Length >= 2)
                            {
                                errorSpanStart = snapshot.Length - 2;
                            }
                            else
                            {
                                // TODO: gross.
                                continue;
                            }
                        }

                        // TODO: currently this just highlights the token under the caret when there's an error.
                        // For things like incomplete strings, however, this isn't good enough.. we'll only highlight
                        // the token under the caret instead of the whole string to the end of the line.
                        // We can probably use our lexer here to highlight the entire lexeme.
                        var errorTokenExtent = this.textStructureNavigator.GetExtentOfWord(new SnapshotPoint(snapshot, errorSpanStart));
                        if (errorTokenExtent.IsSignificant)
                        {
                            errorTokenSpan = errorTokenExtent.Span;
                        }
                        else
                        {
                            errorTokenSpan = new SnapshotSpan(snapshot, errorSpanStart, 1);
                        }

                        errorBuilder.Add(
                            new TagSpan<IErrorTag>(new SnapshotSpan(snapshot, errorTokenSpan),
                            new ErrorTag(PredefinedErrorTypeNames.SyntaxError, lineSegments[3])));
                    }
                }
            }
            while (!process.StandardError.EndOfStream);

            // TODO: raise only for changed span.
            this.errorTagSnapshot = errorBuilder.ToImmutable();
            this.TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(snapshot, new Span(0, snapshot.Length))));
        }
    }
}

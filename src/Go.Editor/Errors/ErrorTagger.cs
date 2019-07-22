namespace Go.Editor.Errors
{
    using System;
    using System.Collections.Generic;
    using Go.Editor.Common;
    using Go.Interop;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;

    internal sealed class ErrorTagger : ITagger<IErrorTag>
    {
        private readonly GoWorkspace workspace;
        private readonly ITextBuffer buffer;

        public ErrorTagger(GoWorkspace workspace, ITextBuffer buffer)
        {
            this.workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
            this.buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));

            this.workspace.QueueFileParse("Foo.go", GoSnapshot.FromSnapshot(buffer.CurrentSnapshot.ToSnapshot()));
        }

#pragma warning disable 0067
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
#pragma warning restore 0067

        public IEnumerable<ITagSpan<IErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            // TODO: this needs to respond to text changed.
            // TODO: queue parses and share among features.
            var snapshot = this.buffer.CurrentSnapshot.ToSnapshot();

            var goSnapshot = GoSnapshot.FromSnapshot(snapshot);

            return null;
        }
    }
}

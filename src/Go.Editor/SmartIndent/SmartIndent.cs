namespace Go.Editor.SmartIndent
{
    using System;
    using Go.Editor.Common;
    using Go.Interop.Workspace;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;

    internal sealed class SmartIndent : ISmartIndent
    {
        private readonly WorkspaceDocument<ITextBuffer> document;
        private readonly IEditorOptions options;

        public SmartIndent(WorkspaceDocument<ITextBuffer> document, IEditorOptions options)
        {
            this.document = document ?? throw new ArgumentNullException(nameof(document));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public void Dispose() => this.document.Dispose();

        public int? GetDesiredIndentation(ITextSnapshotLine line)
        {
            var snapshot = this.document.CurrentSnapshot.ToTextSnapshot();
            var closeBracePosition = this.document.GetPositionOfCloseBraceOfEnclosingBlock(line.Start.Position);
            if (closeBracePosition != -1)
            {
                var closeBraceSnapshotPoint = new SnapshotPoint(snapshot, closeBracePosition);
                var tabSize = this.options.GetOptionValue(DefaultOptions.TabSizeOptionId);

                return this.ComputeIndent(closeBraceSnapshotPoint.GetContainingLine(), tabSize) + tabSize;
            }

            return null;
        }

        private int ComputeIndent(ITextSnapshotLine snapshotLine, int tabSize)
        {
            int indent = 0;

            for (int i = snapshotLine.Start; i < snapshotLine.End && char.IsWhiteSpace(snapshotLine.Snapshot[i]); i++)
            {
                var c = snapshotLine.Snapshot[i];
                if (c == ' ')
                {
                    indent++;
                }
                else if (c == '\t')
                {
                    // TODO: not strictly correct, tabs are 'stops' that can shrink when preceded by non-whitespace.
                    indent += tabSize;
                }
            }

            return indent;
        }
    }
}

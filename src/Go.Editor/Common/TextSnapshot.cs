namespace Go.Editor.Common
{
    using System;
    using Go.Interop.Text;
    using Microsoft.VisualStudio.Text;

    internal sealed class TextSnapshot : SnapshotBase
    {
        public TextSnapshot(ITextSnapshot snapshot) : base(snapshot.Length)
        {
            this.Snapshot = snapshot
                ?? throw new ArgumentNullException(nameof(snapshot));
        }

        public override char this[int offset] => this.Snapshot[offset];

        public ITextSnapshot Snapshot { get; }
    }
}

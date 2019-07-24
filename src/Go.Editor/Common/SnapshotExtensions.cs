namespace Go.Editor.Common
{
    using System;
    using Go.Interop.Text;
    using Microsoft.VisualStudio.Text;

    internal static class SnapshotExtensions
    {
        public static SnapshotBase ToSnapshot(this ITextSnapshot snapshot) => new TextSnapshot(snapshot);

        public static ITextSnapshot ToTextSnapshot(this SnapshotBase snapshot)
        {
            if (snapshot is TextSnapshot textSnapshot)
            {
                return textSnapshot.Snapshot;
            }

            throw new ArgumentException($"{nameof(snapshot)} must be of type {nameof(TextSnapshot)}");
        }

        public static SnapshotSegment ToSegment(this SnapshotSpan snapshotSpan)
        {
            return new SnapshotSegment(snapshotSpan.Snapshot.ToSnapshot(), snapshotSpan.Start, snapshotSpan.Length);
        }

        public static SnapshotSpan ToSnapshotSpan(this SnapshotSegment segment)
        {
            return new SnapshotSpan(segment.Snapshot.ToTextSnapshot(), segment.Start, segment.Length);
        }
    }
}

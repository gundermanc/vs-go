namespace Go.CodeAnalysis
{
    using System;

    public struct SnapshotSegment
    {
        public SnapshotSegment(SnapshotBase snapshotBase, int start, int length)
        {
            this.Snapshot = snapshotBase
                ?? throw new ArgumentNullException(nameof(snapshotBase));

            if (start < 0 || start >= snapshotBase.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(start));
            }

            this.Start = start;

            if ((start + length) > snapshotBase.Length || length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            this.Length = length;
        }

        public SnapshotBase Snapshot { get; }

        public char this[int offset] => Snapshot[this.Start + offset];

        public int Start { get; }

        public int Length { get; }
    }
}

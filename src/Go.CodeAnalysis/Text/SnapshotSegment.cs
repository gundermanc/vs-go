namespace Go.CodeAnalysis.Text
{
    using System;
    using System.Text;

    public struct SnapshotSegment : IEquatable<string>, IEquatable<SnapshotSegment>
    {
        public SnapshotSegment(SnapshotBase snapshotBase, int start, int length)
        {
            this.Snapshot = snapshotBase
                ?? throw new ArgumentNullException(nameof(snapshotBase));

            if (start < 0 || start > snapshotBase.Length)
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

        public char this[int offset] => this.Snapshot[this.Start + offset];

        public int Start { get; }

        public int Length { get; }

        public int End => this.Start + this.Length;

        public string GetText()
        {
            if (this.Length == 0)
            {
                return string.Empty;
            }

            var stringBuilder = new StringBuilder();
            for (int i = 0; i < this.Length; i++)
            {
                stringBuilder.Append(this[i]);
            }
            return stringBuilder.ToString();
        }

        public bool TryGetSingleChar(out char c)
        {
            if (this.Length == 1)
            {
                c = this[0];
                return true;
            }

            c = default;
            return false;
        }

        public bool Equals(string text)
        {
            if (this.Length != text.Length)
            {
                return false;
            }

            for (int i = 0; i < this.Length; i++)
            {
                if (this[i] != text[i])
                {
                    return false;
                }
            }

            return true;
        }

        public bool Equals(SnapshotSegment other)
        {
            return this.Snapshot == other.Snapshot &&
                this.Start == other.Start &&
                this.Length == other.Length;
        }

        public override string ToString() => $"[{this.Start}, {this.End}): {this.GetText()}";
    }
}

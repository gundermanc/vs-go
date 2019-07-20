namespace Go.Interop.Text
{
    using System;
    using System.Text;

    /// <summary>
    /// Represents a span of code.
    /// </summary>
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

        /// <summary>
        /// Starting position of the code span.
        /// </summary>
        public int Start { get; }

        /// <summary>
        /// Length of the code span.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Ending position of the code span.
        /// This is the position after the last charater position in the span.
        /// </summary>
        public int End => this.Start + this.Length;

        /// <summary>
        /// Returns whether a specific position is located within the code segment.
        /// </summary>
        /// <param name="position"> A given position. </param>
        /// <returns> True if the position is within the code segment. </returns>
        public bool ContainsPosition(int position) => this.Start <= position && this.End > position;

        /// <summary>
        /// Returns the copy of string of this code segment.
        /// </summary>
        /// <returns> A string copy of this code segment. </returns>
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

        /// <summary>
        /// Returns the character representation if this code span contains only one character.
        /// </summary>
        /// <param name="c"> A character copy of this code span. </param>
        /// <returns> True if this code span contains only one character. </returns>
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

        /// <summary>
        /// Returns whether the code span equals to a specific string.
        /// </summary>
        /// <param name="text"> A string to be compared to this code span. </param>
        /// <returns> True if the string equals to the code span text. </returns>
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

        /// <summary>
        /// Returns whether this code span equals to a specific code span.
        /// </summary>
        /// <param name="other"> A code segment to be compared with this code span. </param>
        /// <returns> True if two code spans are the same. </returns>
        public bool Equals(SnapshotSegment other)
        {
            return this.Snapshot == other.Snapshot &&
                this.Start == other.Start &&
                this.Length == other.Length;
        }

        public override string ToString() => $"[{this.Start}, {this.End}): {this.GetText()}";
    }
}

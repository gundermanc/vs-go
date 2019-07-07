namespace Go.CodeAnalysis.Text
{
    public abstract class SnapshotBase
    {
        protected SnapshotBase(int length)
        {
            this.Length = length;
        }

        public abstract char this[int offset] { get; }

        public SnapshotSegment Extent => new SnapshotSegment(this, 0, this.Length);

        public int Length { get; }
    }
}

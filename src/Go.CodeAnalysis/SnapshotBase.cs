namespace Go.CodeAnalysis
{
    public abstract class SnapshotBase
    {
        protected SnapshotBase(int length)
        {
            this.Length = length;
        }

        public abstract char this[int offset] { get; }

        public int Length { get; }
    }
}

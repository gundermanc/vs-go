namespace Go.Interop
{
    public struct TypedToken
    {
        public TypedToken(int pos, int end, TokenType type)
        {
            this.Pos = pos;
            this.End = end;
            this.Type = type;
        }

        public int Pos { get; }

        public int End { get; }

        public TokenType Type { get; }
    }
}

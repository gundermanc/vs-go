namespace Go.CodeAnalysis.Parser
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Go.CodeAnalysis.Common;
    using Go.CodeAnalysis.Lex;
    using Go.CodeAnalysis.Text;

    public class ImportsNode : ParseNode
    {
        public ImportsNode(SnapshotSegment extent, ImmutableArray<ImportNode> imports)
            : base(extent, imports.CastArray<ParseNode>())
        {
            this.Imports = imports;
        }

        public ImmutableArray<ImportNode> Imports { get; }

        public static bool TryParse(Lexer lexer, IList<Error> errors, out ImportsNode parseNode)
        {
            var start = lexer.CurrentLexeme.Extent.Start;

            var importsBuilder = ImmutableArray.CreateBuilder<ImportNode>();
            while (lexer.CurrentLexeme.Type == LexemeType.Keyword &&
                lexer.CurrentLexeme.Extent.Equals(Keywords.Import))

            {
                if (!ImportNode.TryParse(lexer, errors, out var importNode))
                {
                    parseNode = null;
                    return false;
                }

                if (!lexer.TryAdvanceLexemeOrReportError(errors) ||
                    !lexer.IsCorrectLexemeTypeOrReportError(LexemeType.Semicolon, errors) ||
                    !lexer.TryAdvanceLexemeOrReportError(errors))
                {
                    parseNode = null;
                    return false;
                }

                importsBuilder.Add(importNode);
            }

            var extent = new SnapshotSegment(lexer.Snapshot, start, lexer.CurrentLexeme.Extent.End - start);

            parseNode = new ImportsNode(extent, importsBuilder.ToImmutable());
            return true;
        }
    }
}

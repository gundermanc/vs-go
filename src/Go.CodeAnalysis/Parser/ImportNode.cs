namespace Go.CodeAnalysis.Parser
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Go.CodeAnalysis.Common;
    using Go.CodeAnalysis.Lex;
    using Go.CodeAnalysis.Text;

    /// <summary>
    /// Represents a single import line or import block.
    /// </summary>
    public sealed class ImportNode : ParseNode
    {
        public ImportNode(
            SnapshotSegment extent,
            ImmutableArray<ImportDeclarationNode> importDeclarations)
            : base(extent, ImmutableArray<ParseNode>.Empty)
        {
            this.ImportExtent = extent;
            this.ImportDeclarations = importDeclarations;
        }

        public SnapshotSegment ImportExtent { get; }

        public ImmutableArray<ImportDeclarationNode> ImportDeclarations { get; }

        public static bool TryParse(Lexer lexer, IList<Error> errors, out ImportNode parseNode)
        {
            var start = lexer.CurrentLexeme.Extent.Start;

            // Parse "import" keyword.
            if (!lexer.IsCorrectLexemeKeywordOrReportError(Keywords.Import, errors)
                || !lexer.TryAdvanceLexemeOrReportError(errors))
            {
                parseNode = null;
                return false;
            }

            // Single line import (string immediately following import keyword).
            if (!lexer.IsCorrectLexemeOperatorOrReportError('(', errors))
            {
                // try parse import declaration here

                var singleLineImportExtent = new SnapshotSegment(lexer.Snapshot, start, lexer.CurrentLexeme.Extent.End - start);
                parseNode = new ImportNode(
                    singleLineImportExtent,
                    ImmutableArray.Create(new ImportDeclarationNode(singleLineImportExtent, string.Empty, string.Empty)));
                return true;
            }

            // Block import.

            // Advance over opening parenthesis.
            if (!lexer.TryAdvanceLexemeOrReportError(errors))
            {
                parseNode = null;
                return false;
            }

            var declarationArrayBuilder = ImmutableArray.CreateBuilder<ImportDeclarationNode>();

            // Parse each import declaration.
            while(!lexer.IsCorrectLexemeOperatorOrReportError(')', errors))
            {
                // try parse import declaration
            }

            var blockImportExtent = new SnapshotSegment(lexer.Snapshot, start, lexer.CurrentLexeme.Extent.End - start);
            parseNode = new ImportNode(blockImportExtent, declarationArrayBuilder.ToImmutable());
            return false;
        }
    }
}

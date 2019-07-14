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
            parseNode = null;

            // Parse "import" keyword.
            if (!lexer.IsCorrectLexemeKeywordOrReportError(Keywords.Import, errors)
                || !lexer.TryAdvanceLexemeOrReportError(errors))
            {
                errors.Add(new Error(lexer.CurrentLexeme.Extent, "Import keyword expected."));
                return false;
            }

            // Single line import (string immediately following import keyword).
            if (!lexer.IsCorrectLexemeOperatorOrReportError('(', errors))
            {
                ImportDeclarationNode singleLineDeclaration = null;
                if (ImportDeclarationNode.TryParse(lexer, errors, out singleLineDeclaration))
                {
                    var singleLineImportExtent = new SnapshotSegment(lexer.Snapshot, start, lexer.CurrentLexeme.Extent.End - start);
                    parseNode = new ImportNode(
                        singleLineImportExtent,
                        ImmutableArray.Create(singleLineDeclaration));
                    return true;
                }
                else
                {
                    return false;
                }
            }

            // Block import.

            // Advance over opening parenthesis.
            if (!lexer.TryAdvanceLexemeOrReportError(errors))
            {
                return false;
            }

            var declarationArrayBuilder = ImmutableArray.CreateBuilder<ImportDeclarationNode>();
            bool isEndOfImportBlock = false;

            // Parse each import declaration.
            while(!isEndOfImportBlock)
            {
                // Parse declaration and advance over.
                ImportDeclarationNode importLineNode = null;
                if (ImportDeclarationNode.TryParse(lexer, errors, out importLineNode)
                    && lexer.TryAdvanceLexemeOrReportError(errors))
                {
                    declarationArrayBuilder.Add(importLineNode);
                }
                else
                {
                    return false;
                }

                if (lexer.IsCorrectLexemeOperatorOrReportError(',', errors)
                    && lexer.TryAdvanceLexemeOrReportError(errors))
                {
                    // If declaration is followed by comma, advance over comma and parse next declaration.
                    continue;
                }
                else if (lexer.IsCorrectLexemeOperatorOrReportError(')', errors))
                {
                    // If declaratino is followed by closing parenthesis, stop parsing.
                    isEndOfImportBlock = true;
                }
                else
                {
                    errors.Add(new Error(lexer.CurrentLexeme.Extent, "',' or ')' expected."));
                    return false;
                }
            }

            var blockImportExtent = new SnapshotSegment(lexer.Snapshot, start, lexer.CurrentLexeme.Extent.End - start);
            parseNode = new ImportNode(blockImportExtent, declarationArrayBuilder.ToImmutable());
            return true;
        }
    }
}

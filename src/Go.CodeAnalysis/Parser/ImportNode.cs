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
    public class ImportNode : ParseNodeBase
    {
        public ImportNode(
            SnapshotSegment extent,
            ImmutableArray<ImportDeclarationNode> importDeclarations)
            : base(extent)
        {
            this.ImportDeclarations = importDeclarations;
        }

        /// <summary>
        /// All imported packages under this import line or block.
        /// </summary>
        public ImmutableArray<ImportDeclarationNode> ImportDeclarations { get; }

        public override void Accept(IVisitor visitor) => visitor.Visit(this);

        public static bool TryParse(Lexer lexer, IList<Error> errors, out ImportNode parseNode)
        {
            var start = lexer.CurrentLexeme.Extent.Start;
            parseNode = null;

            // Parse "import" keyword.
            if (!lexer.IsCorrectLexemeKeywordOrReportError(Keywords.Import, errors)
                || !lexer.TryAdvanceLexemeOrReportError(errors))
            {
                return false;
            }

            // Single line import (string immediately following import keyword).
            if (!lexer.CurrentLexeme.Extent.TryGetSingleChar(out var c)
                || c != '(')
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

                if (lexer.CurrentLexeme.Type == LexemeType.Semicolon)
                {
                    // If declaration is followed by semicolon, advance over it and parse next declaration.
                    if(lexer.TryAdvanceLexeme())
                    {
                        // If semicolon is followed by a closing parenthesis, stop parsing.
                        if (lexer.CurrentLexeme.Extent.TryGetSingleChar(out c) && c == ')')
                        {
                            isEndOfImportBlock = true;
                        }

                        continue;
                    }

                    return false;
                }
                else if (lexer.CurrentLexeme.Extent.TryGetSingleChar(out c) && c == ')')
                {
                    // If declaratino is followed by closing parenthesis, stop parsing.
                    isEndOfImportBlock = true;
                }
                else
                {
                    errors.Add(new Error(
                        lexer.CurrentLexeme.Extent,
                        string.Format(
                            Strings.Error_UnexpectedOperator,
                            lexer.CurrentLexeme.Extent.GetText(),
                            "';' or ')'")));
                    return false;
                }
            }

            var blockImportExtent = new SnapshotSegment(lexer.Snapshot, start, lexer.CurrentLexeme.Extent.End - start);
            parseNode = new ImportNode(blockImportExtent, declarationArrayBuilder.ToImmutable());
            return true;
        }
    }
}

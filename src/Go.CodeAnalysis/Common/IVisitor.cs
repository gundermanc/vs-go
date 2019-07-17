namespace Go.CodeAnalysis.Common
{
    using Go.CodeAnalysis.Parser;

    public interface IVisitor
    {
        void Visit(DocumentNode parseNode);

        void Visit(PackageDeclarationNode parseNode);

        void Visit(ImportsNode parseNode);

        void Visit(ImportDeclarationNode parseNode);

        void Visit(ImportNode parseNode);

        void Visit(DocumentBodyNode parseNode);

        void Visit(FunctionDeclarationNode parseNode);

        void Visit(BlockNode parseNode);
    }
}

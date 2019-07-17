namespace Go.CodeAnalysis.Tests.Semantic
{
    using Go.CodeAnalysis.Parser;
    using Go.CodeAnalysis.Semantic;
    using Go.CodeAnalysis.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class SemanticSnapshotTests
    {
        [TestMethod]
        [Description("Validate some trivial elements of semantic tree")]
        public void SemanticSnapshot_TopLevelConstructs_Basic()
        {
            var snapshot = new StringSnapshot("package main\r\nimport \"yo\"\r\nimport \"mamma\"\r\nfunc main() { }\r\nfunc notmain() { }");
            var parseSnapshot = ParseSnapshot.Create(snapshot);
            var semanticSnapshot = SemanticSnapshot.Create(parseSnapshot);
            Assert.AreEqual(0, semanticSnapshot.Errors.Length);

            Assert.AreEqual("package main", semanticSnapshot.RootNode.PackageDeclaration.Extent.GetText());
            Assert.AreEqual("main", semanticSnapshot.RootNode.PackageDeclaration.PackageNameExtent.GetText());

            Assert.AreEqual(2, semanticSnapshot.RootNode.ImportsNode.Imports.Length);

            Assert.AreEqual("import \"yo\"", semanticSnapshot.RootNode.ImportsNode.Imports[0].Extent.GetText());
            Assert.AreEqual("\"yo\"", semanticSnapshot.RootNode.ImportsNode.Imports[0].ImportDeclarations[0].PackageName.GetText());

            Assert.AreEqual("import \"mamma\"", semanticSnapshot.RootNode.ImportsNode.Imports[1].Extent.GetText());
            Assert.AreEqual("\"mamma\"", semanticSnapshot.RootNode.ImportsNode.Imports[1].ImportDeclarations[0].PackageName.GetText());

            Assert.AreEqual("func main() { }\r\nfunc notmain() { }", semanticSnapshot.RootNode.DocumentBody.Extent.GetText());

            Assert.AreEqual("func main() { }", semanticSnapshot.RootNode.DocumentBody.Declarations[0].Extent.GetText());
            Assert.AreEqual("main", ((FunctionDeclarationNode)semanticSnapshot.RootNode.DocumentBody.Declarations[0]).FunctionNameExtent.GetText());
            Assert.AreEqual("{ }", ((FunctionDeclarationNode)semanticSnapshot.RootNode.DocumentBody.Declarations[0]).BlockNode.Extent.GetText());

            Assert.AreEqual("func notmain() { }", semanticSnapshot.RootNode.DocumentBody.Declarations[1].Extent.GetText());
            Assert.AreEqual("notmain", ((FunctionDeclarationNode)semanticSnapshot.RootNode.DocumentBody.Declarations[1]).FunctionNameExtent.GetText());
            Assert.AreEqual("{ }", ((FunctionDeclarationNode)semanticSnapshot.RootNode.DocumentBody.Declarations[1]).BlockNode.Extent.GetText());
        }
    }
}

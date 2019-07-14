namespace Go.CodeAnalysis.Tests.Parser
{
    using Go.CodeAnalysis.Parser;
    using Go.CodeAnalysis.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class ParseSnapshotTests
    {
        [TestMethod]
        public void ParseSnapshot_EmptyString_NoChildren()
        {
            var snapshot = new StringSnapshot(string.Empty);
            var parseSnapshot = ParseSnapshot.Create(snapshot);
            Assert.AreEqual("Unexpected end of file", parseSnapshot.Errors[0].Message);
            Assert.AreEqual(snapshot.Extent, parseSnapshot.Errors[0].Extent);
        }

        [TestMethod]
        public void ParseSnapshot_IncompletePackageDeclaration_ReturnsError()
        {
            var snapshot = new StringSnapshot("package\r\nimport \"fmt\"");
            var parseSnapshot = ParseSnapshot.Create(snapshot);
            Assert.AreEqual("Unexpected lexeme type Keyword. Expected lexeme type Identifier.", parseSnapshot.Errors[0].Message);
            Assert.AreEqual(new SnapshotSegment(snapshot, 9, 6), parseSnapshot.Errors[0].Extent);
        }

        [TestMethod]
        public void ParseSnapshot_NoPackageDeclaration_ReturnsAnErrors()
        {
            var snapshot = new StringSnapshot("import \"fmt\"");
            var parseSnapshot = ParseSnapshot.Create(snapshot);
            Assert.AreEqual("Unexpected keyword import. Expected package.", parseSnapshot.Errors[0].Message);
            Assert.AreEqual(new SnapshotSegment(snapshot, 0, 6), parseSnapshot.Errors[0].Extent);
        }

        [TestMethod]
        public void ParseSnapshot_PackageDeclaration_ReturnsCompletedNode()
        {
            var snapshot = new StringSnapshot("package main");
            var parseSnapshot = ParseSnapshot.Create(snapshot);
            Assert.AreEqual(0, parseSnapshot.Errors.Length);
            Assert.AreEqual(snapshot.Extent, parseSnapshot.RootNode.Extent);
            Assert.AreEqual(snapshot.Extent, parseSnapshot.RootNode.PackageDeclaration.Extent);
            Assert.AreEqual(new SnapshotSegment(snapshot, 8, 4), parseSnapshot.RootNode.PackageDeclaration.PackageNameExtent);
        }

        [TestMethod]
        public void ParseSnapshot_FunctionDeclaration_KeywordOnly()
        {
            var snapshot = new StringSnapshot("package main\r\n\r\nfunc");
            var parseSnapshot = ParseSnapshot.Create(snapshot);
            Assert.AreEqual("Unexpected end of file", parseSnapshot.Errors[0].Message);
            Assert.AreEqual(new SnapshotSegment(snapshot, 19, 1), parseSnapshot.Errors[0].Extent);
            Assert.AreEqual(snapshot.Extent, parseSnapshot.RootNode.Extent);
        }

        [TestMethod]
        public void ParseSnapshot_FunctionDeclaration_KeywordAndNameOnly()
        {
            var snapshot = new StringSnapshot("package main\r\n\r\nfunc main");
            var parseSnapshot = ParseSnapshot.Create(snapshot);
            Assert.AreEqual("Unexpected lexeme type Semicolon. Expected lexeme type Operator.", parseSnapshot.Errors[0].Message);
            Assert.AreEqual(new SnapshotSegment(snapshot, 25, 0), parseSnapshot.Errors[0].Extent);
            Assert.AreEqual(snapshot.Extent, parseSnapshot.RootNode.Extent);
        }

        [TestMethod]
        public void ParseSnapshot_FunctionDeclaration_UpToOpenParen()
        {
            var snapshot = new StringSnapshot("package main\r\n\r\nfunc main(");
            var parseSnapshot = ParseSnapshot.Create(snapshot);
            Assert.AreEqual("Unexpected end of file", parseSnapshot.Errors[0].Message);
            Assert.AreEqual(new SnapshotSegment(snapshot, 25, 1), parseSnapshot.Errors[0].Extent);
            Assert.AreEqual(snapshot.Extent, parseSnapshot.RootNode.Extent);
        }

        [TestMethod]
        public void ParseSnapshot_FunctionDeclaration_UpToCloseParen()
        {
            var snapshot = new StringSnapshot("package main\r\n\r\nfunc main()");
            var parseSnapshot = ParseSnapshot.Create(snapshot);
            Assert.AreEqual("Unexpected lexeme type Semicolon. Expected lexeme type Operator.", parseSnapshot.Errors[0].Message);
            Assert.AreEqual(new SnapshotSegment(snapshot, 27, 0), parseSnapshot.Errors[0].Extent);
            Assert.AreEqual(snapshot.Extent, parseSnapshot.RootNode.Extent);
        }

        [TestMethod]
        public void ParseSnapshot_FunctionDeclaration_UpToOpenBrace()
        {
            var snapshot = new StringSnapshot("package main\r\n\r\nfunc main() {");
            var parseSnapshot = ParseSnapshot.Create(snapshot);
            Assert.AreEqual("Unexpected end of file", parseSnapshot.Errors[0].Message);
            Assert.AreEqual(new SnapshotSegment(snapshot, 28, 1), parseSnapshot.Errors[0].Extent);
            Assert.AreEqual(snapshot.Extent, parseSnapshot.RootNode.Extent);
        }

        [TestMethod]
        public void ParseSnapshot_FunctionDeclaration_Basic()
        {
            var snapshot = new StringSnapshot("package main\r\n\r\nfunc main() { }");
            var parseSnapshot = ParseSnapshot.Create(snapshot);
            Assert.AreEqual(0, parseSnapshot.Errors.Length);
            Assert.AreEqual(1, parseSnapshot.RootNode.DocumentBody.Children.Length);
        }

        [TestMethod]
        public void ParseSnapshot_FunctionDeclaration_Basic_UnixLineEnding()
        {
            var snapshot = new StringSnapshot("package main\n\nfunc main() { }");
            var parseSnapshot = ParseSnapshot.Create(snapshot);
            Assert.AreEqual(0, parseSnapshot.Errors.Length);
            Assert.AreEqual(1, parseSnapshot.RootNode.DocumentBody.Children.Length);
        }

        [TestMethod]
        [Description("Ensure errors are not generated with comments interleaved in code.")]
        public void ParseSnapshot_FunctionDeclaration_Basic_InterleavedComments()
        {
            var snapshot = new StringSnapshot(@"
// Line
/* General */
package main

// Line
/* General */

func main() {

// Line
/* General */

}

// Line
/* General */
");
            var parseSnapshot = ParseSnapshot.Create(snapshot);
            Assert.AreEqual(0, parseSnapshot.Errors.Length);
            Assert.AreEqual(1, parseSnapshot.RootNode.DocumentBody.Children.Length);
        }

        [TestMethod]
        [Description("Ensure error is thrown if brace in function is on next line")]
        public void ParseSnapshot_FunctionDeclaration_Basic_WrappedBrace()
        {
            var snapshot = new StringSnapshot(@"
package main

func main()
{

}
");
            var parseSnapshot = ParseSnapshot.Create(snapshot);
            Assert.AreEqual("Unexpected lexeme type Semicolon. Expected lexeme type Operator.", parseSnapshot.Errors[0].Message);
            Assert.AreEqual(new SnapshotSegment(snapshot, 30, 0), parseSnapshot.Errors[0].Extent);
        }

        [TestMethod]
        [Description("Ensure proper AST with multiple elements")]
        public void ParseSnapshot_DocumentBody_MultipleElements()
        {
            var snapshot = new StringSnapshot(@"
package main

func main() { }

func notmain() { }
");
            var parseSnapshot = ParseSnapshot.Create(snapshot);
            Assert.AreEqual(0, parseSnapshot.Errors.Length);
        }

        [TestMethod]
        [Description("Validate some trivial elements of parse tree")]
        public void ParseSnapshot_TopLevelConstructs_Basic()
        {
            var snapshot = new StringSnapshot("package main\r\nimport \"yo\"\r\nimport \"mamma\"\r\nfunc main() { }\r\nfunc notmain() { }");
            var parseSnapshot = ParseSnapshot.Create(snapshot);
            Assert.AreEqual(0, parseSnapshot.Errors.Length);

            Assert.AreEqual("package main", parseSnapshot.RootNode.PackageDeclaration.Extent.GetText());
            Assert.AreEqual("main", parseSnapshot.RootNode.PackageDeclaration.PackageNameExtent.GetText());

            Assert.AreEqual(2, parseSnapshot.RootNode.ImportsNode.Children.Length);
            Assert.AreEqual(2, parseSnapshot.RootNode.ImportsNode.Imports.Length);

            Assert.AreEqual("import \"yo\"", parseSnapshot.RootNode.ImportsNode.Imports[0].Extent.GetText());
            Assert.AreEqual("\"yo\"", parseSnapshot.RootNode.ImportsNode.Imports[0].ImportDeclarations[0].PackageName);

            Assert.AreEqual("import \"mamma\"", parseSnapshot.RootNode.ImportsNode.Imports[1].Extent.GetText());
            Assert.AreEqual("\"mamma\"", parseSnapshot.RootNode.ImportsNode.Imports[1].ImportDeclarations[0].PackageName);

            Assert.AreEqual("func main() { }\r\nfunc notmain() { }", parseSnapshot.RootNode.DocumentBody.Extent.GetText());

            Assert.AreEqual("func main() { }", parseSnapshot.RootNode.DocumentBody.Children[0].Extent.GetText());
            Assert.AreEqual("main", ((FunctionDeclarationNode)parseSnapshot.RootNode.DocumentBody.Children[0]).FunctionNameExtent.GetText());
            Assert.AreEqual(1, parseSnapshot.RootNode.DocumentBody.Children[0].Children.Length);
            Assert.AreEqual("{ }", parseSnapshot.RootNode.DocumentBody.Children[0].Children[0].Extent.GetText());

            Assert.AreEqual("func notmain() { }", parseSnapshot.RootNode.DocumentBody.Children[1].Extent.GetText());
            Assert.AreEqual("notmain", ((FunctionDeclarationNode)parseSnapshot.RootNode.DocumentBody.Children[1]).FunctionNameExtent.GetText());
            Assert.AreEqual(1, parseSnapshot.RootNode.DocumentBody.Children[1].Children.Length);
            Assert.AreEqual("{ }", parseSnapshot.RootNode.DocumentBody.Children[1].Children[0].Extent.GetText());
        }
    }
}

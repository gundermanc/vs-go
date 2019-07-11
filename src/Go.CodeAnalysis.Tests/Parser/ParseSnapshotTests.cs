﻿namespace Go.CodeAnalysis.Tests.Parser
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
        [Description("Ensure proper AST with basic import node")]
        public void ParseSnapshot_ImportNode_Basic()
        {
            var snapshot = new StringSnapshot("package main\r\nimport \"fmt\"func main() { }");
            var parseSnapshot = ParseSnapshot.Create(snapshot);
            Assert.AreEqual(0, parseSnapshot.Errors.Length);
        }
    }
}

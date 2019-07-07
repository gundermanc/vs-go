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
            Assert.AreEqual(0, parseSnapshot.RootNode.Children.Length);
            Assert.AreEqual("Unexpected end of file", parseSnapshot.Errors[0].Message);
            Assert.AreEqual(snapshot.Extent, parseSnapshot.Errors[0].Extent);
            Assert.AreEqual(0, parseSnapshot.RootNode.Children.Length);
        }

        [TestMethod]
        public void ParseSnapshot_IncompletePackageDeclaration_ReturnsError()
        {
            var snapshot = new StringSnapshot("package\r\nimport \"fmt\"");
            var parseSnapshot = ParseSnapshot.Create(snapshot);
            Assert.AreEqual(0, parseSnapshot.RootNode.Children.Length);
            Assert.AreEqual("Unexpected lexeme type Keyword. Expected lexeme type Identifier.", parseSnapshot.Errors[0].Message);
            Assert.AreEqual(new SnapshotSegment(snapshot, 9, 6), parseSnapshot.Errors[0].Extent);
            Assert.AreEqual(0, parseSnapshot.RootNode.Children.Length);
        }

        [TestMethod]
        public void ParseSnapshot_NoPackageDeclaration_ReturnsAnErrors()
        {
            var snapshot = new StringSnapshot("import \"fmt\"");
            var parseSnapshot = ParseSnapshot.Create(snapshot);
            Assert.AreEqual("Unexpected keyword import. Expected package.", parseSnapshot.Errors[0].Message);
            Assert.AreEqual(new SnapshotSegment(snapshot, 0, 6), parseSnapshot.Errors[0].Extent);
            Assert.AreEqual(0, parseSnapshot.RootNode.Children.Length);
        }

        [TestMethod]
        public void ParseSnapshot_PackageDeclaration_ReturnsCompletedNode()
        {
            var snapshot = new StringSnapshot("package main");
            var parseSnapshot = ParseSnapshot.Create(snapshot);
            Assert.AreEqual(0, parseSnapshot.Errors.Length);
            Assert.AreEqual(0, parseSnapshot.RootNode.Children.Length);
            Assert.AreEqual(snapshot.Extent, parseSnapshot.RootNode.Extent);
            Assert.AreEqual(snapshot.Extent, parseSnapshot.RootNode.PackageDeclaration.Extent);
            Assert.AreEqual(new SnapshotSegment(snapshot, 8, 4), parseSnapshot.RootNode.PackageDeclaration.PackageNameExtent);
        }
    }
}

namespace Go.CodeAnalysis.Tests.Parser
{
    using Go.CodeAnalysis.Parser;
    using Go.CodeAnalysis.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ParseImportTests
    {
        [TestMethod]
        [Description("Ensure error if missing an end line after import")]
        public void ParseSnapshot_ImportNode_MissingEndLine()
        {
            var snapshot = new StringSnapshot("package main\r\nimport \"fmt\"func main() { }");
            var parseSnapshot = ParseSnapshot.Create(snapshot);
            Assert.AreEqual(1, parseSnapshot.Errors.Length);
            Assert.AreEqual("Unexpected lexeme type Keyword. Expected lexeme type Semicolon.", parseSnapshot.Errors[0].Message);
            Assert.AreEqual(new SnapshotSegment(snapshot, 26, 4), parseSnapshot.Errors[0].Extent);
        }

        [TestMethod]
        [Description("Ensure proper AST with basic import nodes")]
        public void ParseSnapshot_ImportNode_Basic()
        {
            var snapshot = new StringSnapshot("package main\r\nimport \"yo\"\r\nimport \"mamma\"\r\nfunc main() { }");
            var parseSnapshot = ParseSnapshot.Create(snapshot);
            Assert.AreEqual(0, parseSnapshot.Errors.Length);
        }

        [TestMethod]
        [Description("Ensure proper AST with block import nodes")]
        public void ParseSnapshot_ImportNode_Block()
        {
            var snapshot = new StringSnapshot("package main\r\nimport(\"yo\";\"mamma\")\r\nfunc main() { }");
            var parseSnapshot = ParseSnapshot.Create(snapshot);
            Assert.AreEqual(0, parseSnapshot.Errors.Length);
        }

        [TestMethod]
        [Description("Ensure proper AST with multi-line block import nodes")]
        public void ParseSnapshot_ImportNode_MultiLineBlock()
        {
            var snapshot = new StringSnapshot("package main\r\nimport (\r\n\"yo\"\r\n\"mamma\"\r\n)\r\nfunc main() { }");
            var parseSnapshot = ParseSnapshot.Create(snapshot);
            Assert.AreEqual(0, parseSnapshot.Errors.Length);
        }
    }
}

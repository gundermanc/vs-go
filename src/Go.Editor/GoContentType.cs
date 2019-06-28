namespace Go.Editor
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Utilities;

    internal sealed class GoContentType
    {
        public const string Name = "Go";

        [Export]
        [ContentType(Name)]
        [FileExtension(".go")]
        public static readonly FileExtensionToContentTypeDefinition GoFileExtensionMapping = null;

        [Export]
        [Name(Name)]
        [BaseDefinition("code")]
        public static readonly ContentTypeDefinition GoContentTypeDefinition = null;
    }
}

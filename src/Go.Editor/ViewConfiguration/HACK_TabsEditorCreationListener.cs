namespace Go.Editor.ViewConfiguration
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// HACK: set the editor's default whitespace mode to TABs for consistency
    /// with 'gofmt' tool's formatting behavior. In the future, this should be
    /// democratized by the file's contents.
    /// </summary>
    [Export(typeof(ITextViewCreationListener))]
    [ContentType(GoContentType.Name)]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    internal sealed class HACK_TabsEditorCreationListener : ITextViewCreationListener
    {
        public void TextViewCreated(ITextView textView)
        {
            textView.Options.SetOptionValue(DefaultOptions.ConvertTabsToSpacesOptionId, false);
        }
    }
}

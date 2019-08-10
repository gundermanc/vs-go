namespace Go.Editor.BraceCompletion
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.BraceCompletion;
    using Microsoft.VisualStudio.Utilities;

    /// <summary>
    /// For now, just auto-complete open and close braces. At some point, it'd be great
    /// to use <see cref="IBraceCompletionContextProvider"/> or <see cref="IBraceCompletionSessionProvider"/>
    /// to customize the behavior and auto-place the caret inside of blocks on 'return'.
    /// </summary>
    [Export(typeof(IBraceCompletionDefaultProvider))]
    [ContentType(GoContentType.Name)]
    [BracePair('(', ')')]
    [BracePair('[', ']')]
    [BracePair('{', '}')]
    internal sealed class BraceCompletionDefaultProvider : IBraceCompletionDefaultProvider
    {
    }
}

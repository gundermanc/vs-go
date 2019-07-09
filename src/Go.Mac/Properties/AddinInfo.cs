using Mono.Addins;
using Mono.Addins.Description;

[assembly: Addin(
    "Go.Mac",
    Namespace = "Go.Mac",
    Version = ThisAssembly.AssemblyFileVersion
)]

[assembly: AddinName("Go lang for Visual Studio")]
[assembly: AddinCategory("IDE extensions")]
[assembly: AddinDescription("Go lang support for Visual Studio")]
[assembly: AddinAuthor("Christian Gunderman")]

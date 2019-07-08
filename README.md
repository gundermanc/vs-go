# Go lang Support for Visual Studio for Windows and Mac
In progress Go language service for Visual Studio.

![image](https://user-images.githubusercontent.com/5387680/60389194-5e446280-9a72-11e9-9269-dfcaaf349514.png)

## Done

### Editor Features
- Very early prototype of colorization via lexing the file.
- Error squiggles (only for some `gofmt.exe` provided errors and supported parse cases).
- Outlining and structure guides (only for supported parse cases).
- Colorized + formatted documentation tooltips via `gogetdoc.exe` (currently requires files to be in GOPATH directory).
- Smart-indent (only for supported parse cases).

### Supported Parse Cases

- Package declarations
- Top level functions with no parameters, no return type, and no body content.

## Getting started
- Install the Go development SDK.
- Install gofmt and gogetdoc.
- Build in Visual Studio 2019 16.1+.
- Copy src/test-fodder to your GOPATH directory.
- Open the file in the experimental VS instance.

## Roadmap

See https://github.com/gundermanc/vs-go/issues/1 for the roadmap discussion.

Contributions and discussion are absolutely welcome :)

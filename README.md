# Go lang Support for Visual Studio for Windows and Mac
[![Build Status](https://gunderman.visualstudio.com/vs-go/_apis/build/status/gundermanc.vs-go?branchName=master)](https://gunderman.visualstudio.com/vs-go/_build/latest?definitionId=1&branchName=master)

In progress Go language service for Visual Studio for Windows and Mac.

![image](https://user-images.githubusercontent.com/5387680/60867836-91a88f00-a1e0-11e9-9dfe-49d95c269a0c.png)

![image](https://user-images.githubusercontent.com/5387680/60389194-5e446280-9a72-11e9-9269-dfcaaf349514.png)

## Done

### Planned Supported platforms:

- Visual Studio 2019 16.1 for Windows
- Visual Studio 2019 8.2.0 Preview for Mac

### How it does/will work
- Language service smarts are written in Go, using the token, parser, and types libraries
  from the Go standard library to lex, parse, and type check the code.
- Editor features talk to this language service through a thin inteop layer built using a
  a combination of PInvoke and a cgo c-shared library with C language bindings for Go.

### Historical Context
Screenshots are from an earlier build which was a full, from-scratch language service for
Go in C#. Since then, I have changed strategies to enable reuse of existing Go libraries.

We're starting at square one.

### Editor Features
- Currently using TextMate for colorization and outlining and structure. Ideally we'll move
  this to using the Go AST or tokens.
- Error squiggles are directly output by the go/parser and go/types libraries as a result of
  parsing and type checking the code.

## Getting started
- Install the Go development SDK.
- Install gofmt and gogetdoc.
- Build in Visual Studio 2019 16.1+.
- Copy src/test-fodder to your GOPATH directory.
- Open the file in the experimental VS instance.

### Windows
- Install Visual Studio 2019 16.1
- Install Go SDK (expects C:\Go)
- Install MinGW32 GCC C/C++ compiler for Windows (expects C:\MinGW)
- Open Go.Windows.sln, rebuild and start 'Visual Studio Extension'

### Mac
- Install VS for Mac 8.2 Preview
- Install Go SDK
- Install XCode/Apple developer tools (needed for gcc).

Contributions and discussion are absolutely welcome :)

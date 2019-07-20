package main

// typedef struct tagFoo {
//
// } Foo;

// #cgo CFLAGS: -g -Wall
// #include "bindings.h"
import "C"

import (
	"fmt"
)

//export GetGoRoot
// Gets the Go install directory path.
func GetGoRoot() int {
	return 123
}

//export CreateSnapshot
func CreateSnapshot(in []byte, out []byte, byteLen uint) {

	C.foo()
	// fset := token.NewFileSet()

	// source := `

	// package main

	// func main() {

	// }`

	// f, err := parser.ParseFile(fset, nil, source, parser.AllErrors)
}

type Snapshot struct {
}

// Entry point for language service test app.
// Note: The "C" library requires there to be main
// to work, however, it's not run in normal circumstances
func main() {
	fmt.Println("Go language service bindings")
	fmt.Println("By: Christian Gunderman")

}

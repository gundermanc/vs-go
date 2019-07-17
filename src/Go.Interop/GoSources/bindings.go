package main

import (
	"C"
	//"runtime"
)
import "fmt"

//export GetGoRoot
// Gets the Go install directory path.
func GetGoRoot() {

}

// Entry point for language service test app.
// Note: The "C" library requires there to be main
// to work, however, it's not run in normal circumstances
func main() {
	fmt.Println("Go language service bindings")
	fmt.Println("By: Christian Gunderman")

}

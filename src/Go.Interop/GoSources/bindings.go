package main

// #cgo CFLAGS: -g -Wall
// #include "bindings.h"
import "C"
import (
	"fmt"
	"io/ioutil"
)

type ReadCallback func(buffer []byte, offset int, count int) int

// type Snapshot struct {
// 	Read   ReadCallback
// 	length int
// }

//export PrintSnapshot
func PrintSnapshot(snapshot C.Snapshot) {

	buffer := make([]byte, 10)

	C.Read(snapshot, (*C.uint8_t)(&buffer[0]), 0, C.int(len(buffer)))

	ioutil.WriteFile("C:\\repos\\vs-go\\out.txt", buffer, 0)
}

// Entry point for language service test app.
// Note: The "C" library requires there to be main
// to work, however, it's not run in normal circumstances
func main() {
	fmt.Println("Go language service bindings")
	fmt.Println("By: Christian Gunderman")
}

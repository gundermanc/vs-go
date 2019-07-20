package main

// #cgo CFLAGS: -g -Wall
// #include "bindings.h"
import "C"
import (
	"fmt"
	"go/parser"
	"go/token"
	"io"
	"io/ioutil"
)

type snapshotReader struct {
	Snapshot C.Snapshot
	Offset   int
}

func newReader(snapshot C.Snapshot) *snapshotReader {
	return &snapshotReader{snapshot, 0}
}

// Read reads bytes from a snapshotReader into a buffer.
func (reader *snapshotReader) Read(buffer []byte) (n int, err error) {

	if reader.Offset >= int(reader.Snapshot.length) {
		return 0, io.EOF
	}

	read := int(C.Read(reader.Snapshot, (*C.uint8_t)(&buffer[0]), C.int(reader.Offset), C.int(len(buffer))))
	reader.Offset += read

	return read, nil
}

//export PrintSnapshot
func PrintSnapshot(snapshot C.Snapshot) {
	fset := token.NewFileSet()

	reader := newReader(snapshot)

	f, err := parser.ParseFile(fset, "", reader, 0)
	if err != nil {
		ioutil.WriteFile("C:\\repos\\vs-go\\out.txt", []byte(err.Error()), 0)
		return
	}

    // TODO: this is here cause I'm not sure how to avoid 'declaring' f.
	f.Pos()

	ioutil.WriteFile("C:\\repos\\vs-go\\out.txt", []byte("Succeeded"), 0)
}

// Entry point for language service test app.
// Note: The "C" library requires there to be main
// to work, however, it's not run in normal circumstances
func main() {
	fmt.Println("Go language service bindings")
	fmt.Println("By: Christian Gunderman")
}

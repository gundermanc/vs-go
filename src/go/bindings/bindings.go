package main

// #cgo CFLAGS: -g -Wall
// #include "bindings.h"
import "C"
import (
	"go/languageservice"
	"io"
	"unsafe"
)

type snapshotReader struct {
	Snapshot C.Snapshot
	Offset   int
}

func (snapshot C.Snapshot) newReader() *snapshotReader {
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

//export CreateNewWorkspace
func CreateNewWorkspace() int32 {
	return int32(languageservice.CreateNewWorkspace())
}

//export RegisterWorkspaceUpdateCallback
func RegisterWorkspaceUpdateCallback(workspaceID int32, callback C.ProvideStringCallback) {

	goCallback := func(fileName string) {
		fileNameSlice := []byte(fileName)
		C.InvokeStringCallback(callback, (*C.uint8_t)(&fileNameSlice[0]), C.int(len(fileNameSlice)))
	}

	languageservice.WorkspaceID(workspaceID).RegisterWorkspaceUpdateCallback(goCallback)
}

//export QueueFileParse
func QueueFileParse(workspaceID int32, fileName *byte, count int32, snapshot C.Snapshot) {
	reader := snapshot.newReader()
	fileNameString := cToString(fileName, count)
	languageservice.WorkspaceID(workspaceID).QueueFileParse(fileNameString, reader)
}

//export GetWorkspaceCompletions
func GetWorkspaceCompletions(workspaceID int, callback C.ProvideStringCallback) {
	completions, _ := languageservice.WorkspaceID(workspaceID).GetCompletions()

	for _, completion := range completions {

		completionText := []byte(completion)

		C.InvokeStringCallback(callback, (*C.uint8_t)(&completionText[0]), C.int(len(completionText)))
	}
}

//export GetWorkspaceErrors
func GetWorkspaceErrors(workspaceID int32, callback C.ProvideStringCallback) {
	rawErrors := languageservice.WorkspaceID(workspaceID).GetWorkspaceErrors()

	for _, err := range rawErrors {

		errorText := []byte(err.Error())

		C.InvokeStringCallback(callback, (*C.uint8_t)(&errorText[0]), C.int(len(errorText)))
	}
}

func cToString(bytes *byte, length int32) string {
	// TODO: there are 2 copies being made here. Eliminate one.
	return string(C.GoBytes(unsafe.Pointer(bytes), C.int(length)))
}

// Note: The "C" library requires there to be main
// to work, however, it's not run in normal circumstances
func main() {
}

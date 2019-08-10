package main

// #cgo CFLAGS: -g -Wall
// #include "bindings.h"
import "C"
import (
	"go/languageservice"
	"io"
	"io/ioutil"
	"os"
	"runtime/debug"
	"strings"
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
	defer logPanics()

	if reader.Offset >= int(reader.Snapshot.length) {
		return 0, io.EOF
	}

	read := int(C.Read(reader.Snapshot, (*C.uint8_t)(&buffer[0]), C.int(reader.Offset), C.int(len(buffer))))
	reader.Offset += read

	return read, nil
}

//export CreateNewWorkspace
func CreateNewWorkspace() int32 {
	defer logPanics()
	return int32(languageservice.CreateNewWorkspace())
}

//export RegisterWorkspaceUpdateCallback
func RegisterWorkspaceUpdateCallback(workspaceID int32, callback C.ProvideStringCallback) {
	defer logPanics()

	goCallback := func(fileName string, versionId uintptr) {
		fileNameSlice := []byte(fileName)
		versionIdPointer := unsafe.Pointer(versionId)
		C.InvokeWorkspaceUpdatedCallback(callback, (*C.uint8_t)(&fileNameSlice[0]), C.int(len(fileNameSlice)), versionIdPointer)
	}

	languageservice.WorkspaceID(workspaceID).RegisterWorkspaceUpdateCallback(goCallback)
}

//export QueueFileParse
func QueueFileParse(workspaceID int32, fileName *byte, count int32, snapshot C.Snapshot, versionId uintptr) {
	defer logPanics()
	reader := snapshot.newReader()
	fileNameString := cToString(fileName, count)
	languageservice.WorkspaceID(workspaceID).QueueFileParse(fileNameString, reader, versionId)
}

//export GetCompletions
func GetCompletions(workspaceID int, fileName *byte, count int32, callback C.ProvideStringCallback, position int) {
	defer logPanics()
	fileNameString := cToString(fileName, count)

	completions, _ := languageservice.WorkspaceID(workspaceID).GetCompletions(fileNameString, position)

	for _, completion := range completions {

		completionText := []byte(completion)

		C.InvokeStringCallback(callback, (*C.uint8_t)(&completionText[0]), C.int(len(completionText)))
	}
}

//export GetTokens
func GetTokens(workspaceID int32, fileName *byte, count int32, callback C.ProvideTokenCallback) {
	defer logPanics()

	fileNameString := cToString(fileName, count)

	tokens, err := languageservice.WorkspaceID(workspaceID).GetTokens(fileNameString)
	if err == nil {
		for _, token := range tokens {
			C.InvokeTokenCallback(callback, C.int32_t(token.Pos), C.int32_t(token.End), C.int32_t(token.Type))
		}
	}
}

//export GetErrors
func GetErrors(workspaceID int32, fileName *byte, count int32, callback C.ProvideStringCallback) {
	defer logPanics()

	fileNameString := cToString(fileName, count)

	rawErrors := languageservice.WorkspaceID(workspaceID).GetErrors(fileNameString)

	for _, err := range rawErrors {

		errorText := []byte(err.Error())

		C.InvokeStringCallback(callback, (*C.uint8_t)(&errorText[0]), C.int(len(errorText)))
	}
}

//export GetQuickInfo
func GetQuickInfo(workspaceID int32, fileName *byte, count int32, offset int32, callback C.ProvideStringCallback) {
	defer logPanics()

	fileNameString := cToString(fileName, count)

	if str, err := languageservice.WorkspaceID(workspaceID).GetQuickInfo(fileNameString, int(offset)); err == nil {
		strBytes := []byte(str)
		C.InvokeStringCallback(callback, (*C.uint8_t)(&strBytes[0]), C.int(len(strBytes)))
	}
}

func cToString(bytes *byte, length int32) string {
	// TODO: there are 2 copies being made here. Eliminate one.
	return string(C.GoBytes(unsafe.Pointer(bytes), C.int(length)))
}

// Another unfortunately unavoidable singleton.
var logFile *os.File

// TODO: post-back log entries to the IDE output pane.
func log(message string) {
	if logFile == nil {
		logDir := os.TempDir()
		tempFile, err := ioutil.TempFile(logDir, "VS-Go*.log")

		if err != nil {
			panic("Failed to generate log file path")
		}

		logFile = tempFile
	}

	messageBytes := []byte(message)

	if !strings.HasSuffix(message, "\r") && !strings.HasSuffix(message, "\n") {
		messageBytes = append(messageBytes, []byte("\r\n")...)
	}

	if _, err := logFile.WriteString(string(messageBytes)); err != nil {
		panic("Failed to write to log")
	}
}

func logPanics() {

	if r := recover(); r != nil {
		if err := r.(error); r != nil {
			log(err.Error())
			log(string(debug.Stack()))
		} else {
			log("Unknown panic type")
		}
	}
}

// Note: The "C" library requires there to be main
// to work, however, it's not run in normal circumstances
func main() {
}

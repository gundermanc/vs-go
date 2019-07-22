// Go lang workspace document
// By: Christian Gunderman

package languageservice

import (
	"go/ast"
	"go/importer"
	"go/parser"
	"go/token"
	"go/types"
	"io"
	"sync"
)

type workspaceDocument struct {
	QueuedReparses int
	Mutex          *sync.Mutex // TODO: consider replacing mutex with channel?
	File           *ast.File
	Error          []error
}

func createNewWorkspaceDocument(fileName string) *workspaceDocument {
	return &workspaceDocument{
		QueuedReparses: 0,
		Mutex:          &sync.Mutex{},
		File:           nil,
		Error:          nil,
	}
}

func (document *workspaceDocument) queueReparse(fileName string, reader io.Reader, callback WorkspaceUpdateCallback) {

	document.Mutex.Lock()
	document.QueuedReparses++
	oldQueuedReparses := document.QueuedReparses
	document.Mutex.Unlock()

	if oldQueuedReparses == 1 {
		go document.reparseAll(fileName, reader, callback)
	}
}

func (document *workspaceDocument) reparseAll(fileName string, reader io.Reader, callback WorkspaceUpdateCallback) {
	for true {

		document.Mutex.Lock()

		oldQueuedReparses := document.QueuedReparses
		document.QueuedReparses = 0

		document.Mutex.Unlock()

		// TODO: consider swapping the mutex for atomic/interlocked operations or channel?
		if oldQueuedReparses > 0 {
			document.reparse(fileName, reader)
			callback(fileName)
		} else {
			return
		}
	}
}

func (document *workspaceDocument) reparse(fileName string, reader io.Reader) {
	if reader == nil {
		panic("reader cannot be nil")
	}

	fileSet := token.NewFileSet()

	// Do parse.
	f, err := parser.ParseFile(fileSet, "", reader, 0)

	// Update document.
	document.Mutex.Lock()
	defer document.Mutex.Unlock()
	if err != nil {
		document.Error = append(document.Error, err)
	}

	document.File = f

	// See https://github.com/golang/example/tree/master/gotypes for full example code.

	// A Config controls various options of the type checker.
	// The defaults work fine except for one setting:
	// we must specify how to deal with imports.
	conf := types.Config{Importer: importer.Default()}

	// Type-check the package containing only file f.
	// Check returns a *types.Package.
	// TODO: at some point, use a snapshot of the workspace
	// so we can resolve types across all files.
	_, err = conf.Check("cmd/hello", fileSet, []*ast.File{f}, nil)
	if err != nil {
		document.Error = append(document.Error, err)
	}
}

// Go lang Workspace
// By: Christian Gunderman

package languageservice

import (
	"errors"
	"io"
    "sync"
)

// Rant: singletons are a terrible anti-pattern that needs to be avoided
// at all costs. However, in this case, we have no alternative because the
// interop necessary to transfer the whole AST to the C# side is too expensive
// and allowing C# to hold onto a Go pointer violates the Go memory model.
// Instead.. We'll identify important items by a handle.

// TODO: figure out synchronization.
var manager = workspaceManager{Workspaces:make(map[WorkspaceID]*workspace), l: new(sync.RWMutex),}
var nextManagerID = 0

// WorkspaceID uniquely identifies a workspace to the caller.
type WorkspaceID int

// WorkspaceUpdateCallback indicates that the specified workspace file was updated.
type WorkspaceUpdateCallback func(fileName string)

type workspaceManager struct {
	Workspaces map[WorkspaceID]*workspace
    l *sync.RWMutex
}

func (w *workspaceManager) Set(key WorkspaceID, value *workspace) {
	w.l.Lock()
	defer w.l.Unlock()
	w.Workspaces[key] = value
}

func (w *workspaceManager) Get(key WorkspaceID) (*workspace, error) {
	w.l.RLock()
	defer w.l.RUnlock()
	id, ok := w.Workspaces[key]
	if !ok {
		return nil, errors.New(" workspace id not found")
	}
	return id, nil
}
func (w *workspaceManager) Delete(key WorkspaceID) {
	w.l.Lock()
	defer w.l.Unlock()
	delete(w.Workspaces, key)
}

type workspace struct {
	Callbacks []WorkspaceUpdateCallback

	// TODO: to enable incremental-ish behavior, I have separate file sets per file.
	// The idea is that we can create a combined file set at a point in time for type
	// checking if needed. Not sure how idomatic this is...
	Files  map[string]*workspaceDocument
	Errors []error
}

// CreateNewWorkspace creates a workspace and returns a unique Id
// for it.
func CreateNewWorkspace() WorkspaceID {
	workspace := workspace{
		Callbacks: nil,
		Files:     make(map[string]*workspaceDocument, 0),
		Errors:    nil,
	}

	// TODO: synchronize these two
	// TODO: there's a hypothetical concern with int wrap around as we open + close workspaces.
	thisManagerID := WorkspaceID(nextManagerID)
	manager.Workspaces[thisManagerID] = &workspace

	nextManagerID++

	return thisManagerID
}

// CloseWorkspace frees an open workspace.
func (id WorkspaceID) CloseWorkspace() {
	delete(manager.Workspaces, id)
}

// QueueFileParse queues the reparse of a file. Reparse is signaled to the caller
// via a WorkspaceUpdateCallback.
func (id WorkspaceID) QueueFileParse(fileName string, reader io.Reader) *error {
	if reader == nil {
		panic("reader cannot be nil")
	}

	workspace, err := id.getWorkspace()
	if err != nil {
		return err
	}

	file, ok := workspace.Files[fileName]
	if !ok {
		file = createNewWorkspaceDocument(fileName)
		workspace.Files[fileName] = file
	}

	// TODO: better done with channels?
	callback := func(fileName string) {
		workspace, err := id.getWorkspace()
		if err == nil {
			for _, callback := range workspace.Callbacks {
				callback(fileName)
			}
		}
	}

	file.queueReparse(fileName, reader, callback)
	return nil
}

// RegisterWorkspaceUpdateCallback registers a method that is invoked on the completion of
// a change to a file in the workspace.
func (id WorkspaceID) RegisterWorkspaceUpdateCallback(callback WorkspaceUpdateCallback) *error {

	workspace, err := id.getWorkspace()
	if err != nil {
		return err
	}

	workspace.Callbacks = append(workspace.Callbacks, callback)
	return nil
}

// GetWorkspaceErrors gets all currently known errors in the workspace.
func (id WorkspaceID) GetWorkspaceErrors() []error {

	workspace, err := id.getWorkspace()
	if err != nil {
		return append([]error(nil), *err)
	}

	errors := make([]error, 0)
	for _, wd := range workspace.Files {
		fileErrors := wd.Error
		errors = append(errors, fileErrors...)
	}

	return errors
}

func (id WorkspaceID) getWorkspace() (*workspace, *error) {

	if workspace, ok := manager.Workspaces[id]; ok {
		return workspace, nil
	}

	err := errors.New("Unknown workspace")
	return nil, &err
}

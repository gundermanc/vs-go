// Go lang Workspace
// By: Christian Gunderman

package languageservice

import (
	"errors"
	"go/parser"
	"go/token"
	"io"
)

// Rant: singletons are a terrible anti-pattern that needs to be avoided
// at all costs. However, in this case, we have no alternative because the
// interop necessary to transfer the whole AST to the C# side is too expensive
// and allowing C# to hold onto a Go pointer violates the Go memory model.
// Instead.. We'll identify important items by a handle.

// TODO: figure out synchronization.
var manager = workspaceManager{make(map[WorkspaceID]*workspace)}
var nextManagerID = 0

// WorkspaceID uniquely identifies a workspace to the caller.
type WorkspaceID int

type workspaceManager struct {
	Workspaces map[WorkspaceID]*workspace
}

type workspace struct {
	FileSet *token.FileSet
	Errors  []error
}

// CreateNewWorkspace creates a workspace and returns a unique Id
// for it.
func CreateNewWorkspace() WorkspaceID {
	files := token.NewFileSet()
	workspace := workspace{files, nil}

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

func (id WorkspaceID) ParseFile(reader io.Reader) *error {

	if reader == nil {
		err := errors.New("Reader cannot be nil")
		return &err
	}

	if workspace, err := id.getWorkspace(); err == nil {
		// TODO: how do we invalidate errors?
		_, err := parser.ParseFile(workspace.FileSet, "", reader, 0)
		if err != nil {
			// TODO: synchronize.
			workspace.Errors = append(workspace.Errors, err)
		}
	} else {
		return err
	}

	return nil
}

// GetWorkspaceErrors gets all currently known errors in the workspace.
func (id WorkspaceID) GetWorkspaceErrors() []error {
	if workspace, err := id.getWorkspace(); err == nil {
		return workspace.Errors
	} else {
		return append([]error(nil), *err)
	}
}

func (id WorkspaceID) getWorkspace() (*workspace, *error) {

	if workspace, ok := manager.Workspaces[id]; ok {
		return workspace, nil
	}

	err := errors.New("Unknown workspace")
	return nil, &err
}

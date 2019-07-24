// Go lang Workspace
// By: Christian Gunderman

package languageservice

import (
	"errors"
	"go/ast"
	"go/token"
	"io"
	"sync"
)

// Rant: singletons are a terrible anti-pattern that needs to be avoided
// at all costs. However, in this case, we have no alternative because the
// interop necessary to transfer the whole AST to the C# side is too expensive
// and allowing C# to hold onto a Go pointer violates the Go memory model.
// Instead.. We'll identify important items by a handle.

var manager = WorkspaceManager{Workspaces: make(map[WorkspaceID]*Workspace), l: new(sync.RWMutex)}
var nextManagerID = 0

// WorkspaceID uniquely identifies a Workspace to the caller.
type WorkspaceID int

// WorkspaceUpdateCallback indicates that the specified workspace file was updated.
type WorkspaceUpdateCallback func(fileName string)

type WorkspaceManager struct {
	Workspaces map[WorkspaceID]*Workspace
	l          *sync.RWMutex
}

func (w *WorkspaceManager) SetManager(key WorkspaceID, value *Workspace) {
	w.l.Lock()
	defer w.l.Unlock()
	w.Workspaces[key] = value
}

func (w *WorkspaceManager) GetManager(key WorkspaceID) (*Workspace, error) {
	w.l.RLock()
	defer w.l.RUnlock()
	id, ok := w.Workspaces[key]
	if !ok {
		return nil, errors.New(" Workspace id not found")
	}
	return id, nil
}
func (w *WorkspaceManager) DeleteManager(key WorkspaceID) {
	w.l.Lock()
	defer w.l.Unlock()
	delete(w.Workspaces, key)
}

type Workspace struct {
	Callbacks []WorkspaceUpdateCallback

	// TODO: to enable incremental-ish behavior, I have separate file sets per file.
	// The idea is that we can create a combined file set at a point in time for type
	// checking if needed. Not sure how idomatic this is...
	Files  map[string]*workspaceDocument
	l      *sync.RWMutex
	Errors []error
}

func (ws *Workspace) SetWorkspaceDocument(key string, value *workspaceDocument) {
	ws.l.Lock()
	defer ws.l.Unlock()
	ws.Files[key] = value
}

func (ws *Workspace) GetWorkspaceDocument(key string) (*workspaceDocument, error) {
	ws.l.RLock()
	defer ws.l.RUnlock()
	document, ok := ws.Files[key]
	if !ok {
		return nil, errors.New(" workspaceDocument not found")
	}
	return document, nil
}
func (ws *Workspace) DeleteWorkspaceDocument(key string) {
	ws.l.Lock()
	defer ws.l.Unlock()
	delete(ws.Files, key)
}

// CreateNewWorkspace creates a Workspace and returns a unique Id
// for it.
func CreateNewWorkspace() WorkspaceID {
	workspace := Workspace{
		Callbacks: nil,
		Files:     make(map[string]*workspaceDocument, 0),
		l:         new(sync.RWMutex),
		Errors:    nil,
	}

	// TODO: there's a hypothetical concern with int wrap around as we open + close workspaces.
	thisManagerID := WorkspaceID(nextManagerID)
	manager.SetManager(thisManagerID, &workspace)

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

func (id WorkspaceID) GetCompletions() ([]string, error) {

	workspace, err := id.getWorkspace()
	if err != nil {
		return nil, *err
	}

	completions := []string(nil)

	// TODO: take into account context.
	// TODO: support locals.
	// TODO: return item type.
	// TODO: support fetching item description.
	for _, wd := range workspace.Files {
		decls := wd.File.Decls
		for _, decl := range decls {
			if funcDecl, ok := decl.(*ast.FuncDecl); ok {
				completions = append(completions, funcDecl.Name.Name)
			} else if genDecl, ok := decl.(*ast.GenDecl); ok {

				// TODO: probably a better way to do this via specs.
				for _, spec := range genDecl.Specs {
					if importSpec, ok := spec.(*ast.ImportSpec); ok && importSpec.Name != nil {
						completions = append(completions, importSpec.Name.Name)
					} else if typeSpec, ok := spec.(*ast.TypeSpec); ok && typeSpec.Name != nil {
						completions = append(completions, typeSpec.Name.Name)
					} else if valueSpec, ok := spec.(*ast.ValueSpec); ok {
						for _, name := range valueSpec.Names {
							if name != nil {
								completions = append(completions, name.Name)
							}
						}
					}
				}
			}
		}
	}

	return completions, nil
}

type TokenType int

const (
	KEYWORD      TokenType = 1
	IDENTIFIER   TokenType = 2
	STRING       TokenType = 3
	RESOLVEDTYPE TokenType = 4
	LITERAL      TokenType = 5
	COMMENT      TokenType = 6
)

type TypedToken struct {
	Pos  token.Pos
	End  token.Pos
	Type TokenType
}

// GetTokens fetches the typed tokens from the file. This capability is
// useful for colorizing text in the editor.
func (id WorkspaceID) GetTokens(fileName string) ([]TypedToken, error) {

	workspace, err := id.getWorkspace()
	if err != nil {
		return nil, *err
	}

	tokens := []TypedToken(nil)

	visitor := func(node ast.Node) bool {

		// TODO: consider reworking in terms of interfaces.
		switch typedNode := node.(type) {
		case *ast.File:
			for _, comment := range typedNode.Comments {
				tokens = append(tokens, TypedToken{comment.Pos(), comment.End(), COMMENT})
			}

			tokens = append(tokens, TypedToken{typedNode.Package, typedNode.Package + 7, KEYWORD})

		case *ast.GenDecl:
			tokens = append(tokens, TypedToken{typedNode.TokPos, typedNode.TokPos + token.Pos(len(typedNode.Tok.String())), KEYWORD})

		case *ast.ReturnStmt:
			tokens = append(tokens, TypedToken{typedNode.Return, typedNode.Return + 6, KEYWORD})

		case *ast.IfStmt:
			tokens = append(tokens, TypedToken{typedNode.If, typedNode.If + 2, KEYWORD})
			if typedNode.Else != nil {
				tokens = append(tokens, TypedToken{typedNode.Else.Pos(), typedNode.Else.Pos() + 4, KEYWORD})
			}

		case *ast.StructType:
			tokens = append(tokens, TypedToken{typedNode.Struct, typedNode.Struct + 5, KEYWORD})

		case *ast.InterfaceType:
			tokens = append(tokens, TypedToken{typedNode.Interface, typedNode.Interface + 8, KEYWORD})

		case *ast.FuncLit:
			tokens = append(tokens, TypedToken{typedNode.Type.Func, typedNode.Type.Func + 4, KEYWORD})

		case *ast.MapType:
			tokens = append(tokens, TypedToken{typedNode.Map, typedNode.Map + 2, KEYWORD})

		case *ast.TypeSpec:
			tokens = append(tokens, TypedToken{typedNode.Name.Pos(), typedNode.Name.End(), RESOLVEDTYPE})

		case *ast.ImportSpec:
			tokens = append(tokens, TypedToken{typedNode.Pos(), typedNode.End(), STRING})

		case *ast.Ident:
			tokens = append(tokens, TypedToken{typedNode.Pos(), typedNode.End(), IDENTIFIER})

		case *ast.FuncDecl:
			tokens = append(tokens, TypedToken{typedNode.Type.Func, typedNode.Type.Func + 4, KEYWORD})
			tokens = append(tokens, TypedToken{typedNode.Name.NamePos, typedNode.Name.End() - 1, RESOLVEDTYPE})
		}

		return true
	}

	if file, ok := workspace.Files[fileName]; ok {
		ast.Inspect(file.File, visitor)
	} else {
		return nil, errors.New("File does not exist in workspace")
	}

	return tokens, nil
}

func (id WorkspaceID) getWorkspace() (*Workspace, *error) {

	if workspace, ok := manager.Workspaces[id]; ok {
		return workspace, nil
	}

	err := errors.New("Unknown workspace")
	return nil, &err
}

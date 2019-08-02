// Go lang Workspace
// By: Christian Gunderman

package languageservice

import (
    "errors"
    "go/ast"
    "go/token"
    "io"
    "math"
    "sync"

    "golang.org/x/tools/go/ast/astutil"
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
type WorkspaceUpdateCallback func(fileName string, versionId uintptr)

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
func (id WorkspaceID) QueueFileParse(fileName string, reader io.Reader, versionId uintptr) *error {
    if reader == nil {
        panic("reader cannot be nil")
    }

    workspace, err := id.getWorkspace()
    if err != nil {
        return &err
    }

    file, ok := workspace.Files[fileName]
    if !ok {
        file = createNewWorkspaceDocument(fileName)
        workspace.Files[fileName] = file
    }

    // TODO: better done with channels?
    callback := func(fileName string, versionId uintptr) {
        workspace, err := id.getWorkspace()
        if err == nil {
            for _, callback := range workspace.Callbacks {
                callback(fileName, versionId)
            }
        }
    }

    file.queueReparse(fileName, reader, callback, versionId)
    return nil
}

// RegisterWorkspaceUpdateCallback registers a method that is invoked on the completion of
// a change to a file in the workspace.
func (id WorkspaceID) RegisterWorkspaceUpdateCallback(callback WorkspaceUpdateCallback) *error {

    workspace, err := id.getWorkspace()
    if err != nil {
        return &err
    }

    workspace.Callbacks = append(workspace.Callbacks, callback)
    return nil
}

// GetErrors gets all currently known errors in the specified file.
func (id WorkspaceID) GetErrors(fileName string) []error {

    workspace, err := id.getWorkspace()
    if err != nil {
        return append([]error(nil), err)
    }

    errs := make([]error, 0)
    if file, ok := workspace.Files[fileName]; ok {
        errs = append(errs, file.Error...)
    } else {
        errs = append(errs, errors.New("Unable to find a file of that name"))
    }

    return errs
}

func (id WorkspaceID) GetCompletions(fileName string, position int) ([]string, error) {

    workspace, err := id.getWorkspace()
    if err != nil {
        return nil, err
    }

    completions := []string(nil)

    for candidateFileName, wd := range workspace.Files {
        var codePos token.Pos
        if candidateFileName == fileName {
            codePos = token.Pos(position)
        } else {
            // If the file names don't match, consider codePos to be past EOF
            // so we don't pick up any local completions.
            codePos = math.MaxInt32
        }

        // current position: =  wd.FileSet.Position(codePos).String()
        enclosingPath, _ := astutil.PathEnclosingInterval(wd.File, codePos, codePos+1)

        for _, enclosingNode := range enclosingPath {
            switch castedNode := enclosingNode.(type) {

            // for function declarations, get all their locals declared before codePos
            case *ast.FuncDecl:
                var v completionsFindingVisitor = completionsFindingVisitor{codePos, true, &completions}
                ast.Walk(v, castedNode.Body)
            // for files get all global declarations
            case *ast.File:
                var v completionsFindingVisitor = completionsFindingVisitor{codePos, false, &completions}
                ast.Walk(v, enclosingNode)
            }
        }
    }

    return completions, nil
}

type completionsFindingVisitor struct {
    SourcePos         token.Pos
    PositionSensitive bool
    Completions       *[]string
}

func (v completionsFindingVisitor) Visit(node ast.Node) ast.Visitor {
    switch nodeCasted := node.(type) {
    case *ast.FuncDecl:
        *v.Completions = append(*v.Completions, nodeCasted.Name.Name)
        return nil // don't go deeper
    case *ast.ValueSpec:
        if v.PositionSensitive && node.Pos() >= v.SourcePos {
            return nil
        }

        for _, name := range nodeCasted.Names {
            if name != nil {
                *v.Completions = append(*v.Completions, name.Name)
            }
        }
        return nil
    case *ast.TypeSpec:
        if nodeCasted.Name != nil {
            *v.Completions = append(*v.Completions, nodeCasted.Name.Name)
            return nil
        }
    case *ast.ImportSpec:
        if nodeCasted.Name != nil {
            *v.Completions = append(*v.Completions, nodeCasted.Name.Name)
            return nil
        } else if nodeCasted.Path != nil {
            *v.Completions = append(*v.Completions, nodeCasted.Path.Value[1:len(nodeCasted.Path.Value)-1])
            return nil
        }
    case *ast.AssignStmt:
        if v.PositionSensitive && node.Pos() >= v.SourcePos {
            return nil
        }

        identifier := nodeCasted.Lhs[0].(*ast.Ident)
        if identifier != nil {
            *v.Completions = append(*v.Completions, identifier.Name)
        }
        return nil
    }
    return v
}

type TokenType int

const (
    KEYWORD = iota
    IDENTIFIER
    STRING
    RESOLVEDTYPE
    LITERAL
    COMMENT
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
        return nil, err
    }

    tokens := []TypedToken(nil)

    visitor := func(node ast.Node) bool {
        nodeType := getNodeType(node)
        nodeType.appendNode(&tokens)

        return true
    }

    if file, ok := workspace.Files[fileName]; ok {
        ast.Inspect(file.File, visitor)
    } else {
        return nil, errors.New("file does not exist in workspace")
    }

    return tokens, nil
}

func (id WorkspaceID) getWorkspace() (*Workspace, error) {
    w, ok := manager.Workspaces[id];
    if !ok {
        return nil, errors.New("unknown workspace")
    }

    return w, nil
}

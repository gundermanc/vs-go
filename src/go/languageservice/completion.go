// Go lang completion
// By: Christian Gunderman

package languageservice

import (
	"go/ast"
	"go/token"
	"math"

	"golang.org/x/tools/go/ast/astutil"
)

// GetCompletions returns completions for the specified file in the workspace at
// a location in the current snapshot.
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

// Go lang quick info
// By: Christian Gunderman

package languageservice

import (
	"errors"
	"fmt"
	"go/ast"
	"go/token"

	"golang.org/x/tools/go/ast/astutil"
)

// GetQuickInfo returns the documentation for a particular syntactic construct.
func (id WorkspaceID) GetQuickInfo(fileName string, position int) (string, error) {

	workspace, err := id.getWorkspace()
	if err != nil {
		return "", err
	}

	codePos := token.Pos(position)

	if file, ok := workspace.Files[fileName]; ok {
		interval, _ := astutil.PathEnclosingInterval(file.File, codePos, codePos)
		if interval != nil {
			for _, enclosingNode := range interval {
				switch concrete := enclosingNode.(type) {
				case *ast.FuncDecl:
					return fmt.Sprintf("%v\n\n%v", concrete.Name.String(), concrete.Doc.Text()), nil
				case *ast.GenDecl:
					// TODO: name/signature/
					if concrete.Doc != nil {
						return concrete.Doc.Text(), nil
					}
				case *ast.Ident:
					if concrete.Obj != nil {
						if declNode, ok := concrete.Obj.Decl.(*ast.FuncDecl); ok {
							return fmt.Sprintf("%v\n\n%v", declNode.Name.String(), declNode.Doc.Text()), nil
						} else if declNode, ok := concrete.Obj.Decl.(*ast.TypeSpec); ok {
							return fmt.Sprintf("%v\n\n%v", declNode.Name.String(), declNode.Doc.Text()), nil
						}
					}
				}
			}

			// No documentation here.
			return "", nil
		}
	}

	return "", errors.New("No open file with the given name")
}

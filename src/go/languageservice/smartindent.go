// Go lang completion
// By: Christian Gunderman

package languageservice

import (
	"go/ast"
	"go/token"

	"golang.org/x/tools/go/ast/astutil"
)

// GetCloseBraceOfEnclosingBlock returns the correct indentation of the selected position in the
// document.
func (id WorkspaceID) GetPositionOfCloseBraceOfEnclosingBlock(fileName string, position int) (int, error) {
	doc, err := id.getWorkspaceDocument(fileName)
	if err != nil {
		return 0, err
	}

	pos := token.Pos(position)

	enclosingPath, _ := astutil.PathEnclosingInterval(doc.File, pos, pos)

	for _, enclosingNode := range enclosingPath {
		if blockStmt, ok := enclosingNode.(*ast.BlockStmt); ok {
			return int(blockStmt.Rbrace), nil
		}
	}

	return -1, nil
}

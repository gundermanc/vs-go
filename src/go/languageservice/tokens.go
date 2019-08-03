// Go lang tokens enumeration
// By: Christian Gunderman

package languageservice

import (
	"errors"
	"go/ast"
	"go/token"
)

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
		return nil, err
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

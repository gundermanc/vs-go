package languageservice

import (
    "go/ast"
    "go/token"
)

type tyNode interface {
    appendNode(tokens *[]TypedToken)
}

type File struct {
    *ast.File
}

func (f *File) appendNode(tokens *[]TypedToken) {
    for _, comment := range f.Comments {
        *tokens = append(*tokens, TypedToken{comment.Pos(), comment.End(), COMMENT})
    }
    *tokens = append(*tokens, TypedToken{f.Package, f.Package + 7, KEYWORD})
}

type GenDecl struct {
    *ast.GenDecl
}

func (g *GenDecl) appendNode(tokens *[]TypedToken) {
    *tokens = append(*tokens, TypedToken{g.TokPos, g.TokPos + token.Pos(len(g.Tok.String())), KEYWORD})
}

type ReturnStmt struct {
    *ast.ReturnStmt
}

func (r *ReturnStmt) appendNode(tokens *[]TypedToken) {
    *tokens = append(*tokens, TypedToken{r.Return, r.Return + 6, KEYWORD})
}

type IfStmt struct {
    *ast.IfStmt
}

func (i *IfStmt) appendNode(tokens *[]TypedToken) {
    *tokens = append(*tokens, TypedToken{i.If, i.If + 2, KEYWORD})
    if i.Else != nil {
        *tokens = append(*tokens, TypedToken{i.Else.Pos(), i.Else.Pos() + 4, KEYWORD})
    }
}

type StructType struct {
    *ast.StructType
}

func (s *StructType) appendNode(tokens *[]TypedToken) {
    *tokens = append(*tokens, TypedToken{s.Struct, s.Struct + 5, KEYWORD})
}

type InterfaceType struct {
    *ast.InterfaceType
}

func (i *InterfaceType) appendNode(tokens *[]TypedToken) {
    *tokens = append(*tokens, TypedToken{i.Interface, i.Interface + 8, KEYWORD})
}

type FuncLit struct {
    *ast.FuncLit
}

func (f *FuncLit) appendNode(tokens *[]TypedToken) {
    *tokens = append(*tokens, TypedToken{f.Type.Func, f.Type.Func + 4, KEYWORD})
}

type MapType struct {
    *ast.MapType
}

func (m *MapType) appendNode(tokens *[]TypedToken) {
    *tokens = append(*tokens, TypedToken{m.Map, m.Map + 2, KEYWORD})
}

type TypeSpec struct {
    *ast.TypeSpec
}

func (t *TypeSpec) appendNode(tokens *[]TypedToken) {
    *tokens = append(*tokens, TypedToken{t.Name.Pos(), t.Name.End(), RESOLVEDTYPE})
}

type ImportSpec struct {
    *ast.ImportSpec
}

func (i *ImportSpec) appendNode(tokens *[]TypedToken) {
    *tokens = append(*tokens, TypedToken{i.Pos(), i.End(), STRING})
}

type Ident struct {
    *ast.Ident
}

func (i *Ident) appendNode(tokens *[]TypedToken) {
    *tokens = append(*tokens, TypedToken{i.Pos(), i.End(), IDENTIFIER})
}

type FuncDecl struct {
    *ast.FuncDecl
}

func (f *FuncDecl) appendNode(tokens *[]TypedToken) {
    *tokens = append(*tokens, TypedToken{f.Type.Func, f.Type.Func + 4, KEYWORD})
    *tokens = append(*tokens, TypedToken{f.Name.NamePos, f.Name.End() - 1, RESOLVEDTYPE})
}

func getNodeType(i interface{}) tyNode {
    var nType tyNode

    switch ty := i.(type) {
    case *ast.File:
        nType = &File{ty}

    case *ast.GenDecl:
        nType = &GenDecl{ty}

    case *ast.ReturnStmt:
        nType = &ReturnStmt{ty}

    case *ast.IfStmt:
        nType = &IfStmt{ty}

    case *ast.StructType:
        nType = &StructType{ty}

    case *ast.InterfaceType:
        nType = &InterfaceType{ty}

    case *ast.FuncLit:
        nType = &FuncLit{ty}

    case *ast.MapType:
        nType = &MapType{ty}

    case *ast.TypeSpec:
        nType = &TypeSpec{ty}

    case *ast.ImportSpec:
        nType = &ImportSpec{ty}

    case *ast.Ident:
        nType = &Ident{ty}

    case *ast.FuncDecl:
        nType = &FuncDecl{ty}

    }
    return nType
}

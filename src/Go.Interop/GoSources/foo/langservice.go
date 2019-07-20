package main

import (
	"fmt"
	"go/parser"
	"go/token"
)

func main() {
	fset := token.NewFileSet()

	source := `

	package main
	
	func main() {
		
	}`

	f, err := parser.ParseFile(fset, "", source, parser.AllErrors)
	if err != nil {
		fmt.Errorf("Error")
	}

	fmt.Printf("Package name: %s", f.Name)
}

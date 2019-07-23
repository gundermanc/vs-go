// Hello-world sample in Go!
// By: Christian Gunderman

// This file's contents demonstrate the extent of GoLang support
// in Go for Visual Studio Extension.

package main

// Introduce a typo to see error squiggles.
import (
	"fmt"
)

// Shows up in completion.
const ConstantlyOutdoingExpectations int = 4

// Also shows up in completion.
type SoStructural struct {

}

// Also shows up in completion.
type AreWeInterfacing interface {

}

// Outlining works here, so you should see structure guides and outlining regions.
func main(foo int) {

	fmt.Println("hello world")
}
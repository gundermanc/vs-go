// Hello-world sample in Go!
// By: Christian Gunderman

// This file's contents demonstrate the extent of GoLang support
// in Go for Visual Studio Extension.

package main

// Introduce a typo to see error squiggles.
// Shows up in completion.
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

const globalX int = 1

// Outlining works here, so you should see structure guides and outlining regions.
func main(foo int) {
	const x int = 7 
	var y int = 8

	// globalX, globalY, x and y are visible here, but z not

	var z int = y

	fmt.Println("hello world", z)
}

const globalY int = 1
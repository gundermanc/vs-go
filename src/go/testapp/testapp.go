package main

import (
	"fmt"
	"go/languageservice"
	"strings"
)

func main() {
	reader := strings.NewReader(`

	package main

	import "fmt"

	// main is a test function
	func main() {
		if true {
			if true {
				fmt.Println(1)
			}
		x := Fooooooooooooooooooooooooooooooooo{}
		}
	}

	// Fooooooooooooooooooooooooooooooooo Println123 Hello!
	type Fooooooooooooooooooooooooooooooooo struct {

	}
	
	`)

	finished := false
	callback := func(fileName string, versionId uintptr) {
		fmt.Println("Finished!")
		finished = true
	}

	workspace := languageservice.CreateNewWorkspace()

	// TODO: this should do the equivalent of 'awaiting a task' in C#.
	// I believe channels are the way but too lazy to figure this out
	// for the test app. In VS, we're a UI app, so we'll stay alive anyways.
	workspace.RegisterWorkspaceUpdateCallback(callback)

	err := workspace.QueueFileParse("foo.go", reader, uintptr(0))

	for !finished {
		// It'd be better to use channels to await this but this is a hacky test only app.
	}

	str, _ := workspace.GetQuickInfo("foo.go", 136)
	fmt.Print(str)

	if err == nil {
		for _, workspaceErr := range workspace.GetErrors("foo.go") {
			fmt.Println(workspaceErr.Error())
		}
	} else {
		fmt.Println(err.Error())
	}

	workspace.CloseWorkspace()
}

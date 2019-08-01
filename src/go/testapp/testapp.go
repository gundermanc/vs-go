package main

import (
    "fmt"
    "strings"

    "vs-go/src/go/languageservice"
)

func main() {
    reader := strings.NewReader(`

	package main

	func main() {

	}
	
	`)

    finished := false
    callback := func(fileName string) {
        fmt.Println("Finished!")
        finished = true
    }

    workspace := languageservice.CreateNewWorkspace()

    // TODO: this should do the equivalent of 'awaiting a task' in C#.
    // I believe channels are the way but too lazy to figure this out
    // for the test app. In VS, we're a UI app, so we'll stay alive anyways.
    workspace.RegisterWorkspaceUpdateCallback(callback)

    err := workspace.QueueFileParse("foo.go", reader)

    for !finished {
        // It'd be better to use channels to await this but this is a hacky test only app.
    }

    if err == nil {
        for _, workspaceErr := range workspace.GetWorkspaceErrors() {
            fmt.Println(workspaceErr.Error())
        }
    } else {
        fmt.Println((*err).Error())
    }

    workspace.CloseWorkspace()
}

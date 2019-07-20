package main

import (
	"fmt"
	"go/languageservice"
	"strings"
)

func main() {
	reader := strings.NewReader(`

	package main

	func main() {

	}
	
	`)

	workspace := languageservice.CreateNewWorkspace()
	err := workspace.ParseFile(reader)

	if err == nil {
		for _, workspaceErr := range workspace.GetWorkspaceErrors() {
			fmt.Println(workspaceErr.Error())
		}
	} else {
		fmt.Println((*err).Error())
	}

	workspace.CloseWorkspace()
}

// TcpDummyClient project main.go
package main

import (
	_ "strings"

	"github.com/lxn/walk"
	. "github.com/lxn/walk/declarative"
)

func main() {
	mw := &MyMainWindow{}

	MainWindow{
		AssignTo: &mw.MainWindow,
		Title:    "Tcp Network DummyClient",
		//MinSize:  Size{300, 600},
		//MaxSize:  Size{300, 600},
		Size:   Size{800, 300},
		Layout: VBox{},
		Children: []Widget{
			GroupBox{
				Title:   "Echo Test",
				MinSize: Size{700, 50},
				MaxSize: Size{700, 50},
				Layout:  HBox{},
				Children: []Widget{
					Label{
						Text: "Server IP",
					},
					TextEdit{},

					Label{
						Text: "Server Port",
					},
					TextEdit{},

					Label{
						Text: "Connection Count",
					},
					TextEdit{},

					PushButton{
						Text: "접속",
						//OnClicked: mw.clicked,
					},

					PushButton{
						Text: "Dis 1",
					},

					PushButton{
						Text: "Dis All",
					},
				},
			},

			GroupBox{
				Title:   "Connect/DisConnect Test",
				MinSize: Size{700, 50},
				MaxSize: Size{700, 50},
				Layout:  HBox{},
				Children: []Widget{
					Label{
						Text: "Interval",
					},

					TextEdit{},

					Label{
						Text: "~",
					},

					TextEdit{},

					Label{
						Text: "ms",
					},

					HSpacer{},

					Label{
						Text: "min",
					},
					TextEdit{},

					Label{
						Text: "~ max",
					},

					TextEdit{},

					PushButton{
						Text: "Start/Stop",
					},
				},
			},

			GroupBox{
				Title:   "현재 상태",
				MinSize: Size{700, 50},
				MaxSize: Size{700, 50},
				Layout:  HBox{},
				Children: []Widget{
					Label{
						Text: "Cur Connection",
					},
					TextEdit{ReadOnly: true},
				},
			},

			/*PushButton{
				Text: "SCREAM",
				OnClicked: func() {
					outTE.SetText(strings.ToUpper(inTE.Text()))
				},
			},*/
		},
	}.Run()
}

type MyMainWindow struct {
	*walk.MainWindow
	//searchBox *walk.LineEdit
	//textArea *walk.TextEdit
	//results *walk.ListBox
}

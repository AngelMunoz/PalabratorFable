module App

open Feliz
open Elmish
open Fable.Core
open Fable.Core.JsInterop

[<Import("TsCounter", "./tsfiles/interop")>]
let TsCounter' (state: {| counter: int |}): Fable.React.ReactElement = jsNative

type State = { Count: int }

type Msg =
    | Increment
    | Decrement

let init() = { Count = 0 }, Cmd.none

let update (msg: Msg) (state: State) =
    match msg with
    | Increment -> { state with Count = state.Count + 1 }, Cmd.none
    | Decrement -> { state with Count = state.Count - 1 }, Cmd.none

let TsCounter (counter: int) =
    Fable.React.FunctionComponent.Of TsCounter' {| counter = counter |}

let render (state: State) (dispatch: Msg -> unit) =
    
    Html.article [
        Html.h1 [ Html.text "Hello Fable and Feliz from F#" ]
        Html.div [
            Html.button [
                prop.onClick (fun _ -> dispatch Increment)
                prop.text "Increment"
            ]

            Html.button [
                prop.onClick (fun _ -> dispatch Decrement)
                prop.text "Decrement"
            ]

            Html.h1 state.Count
        ]
        TsCounter state.Count
    ]

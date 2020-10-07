module User

open Feliz
open Feliz.UseElmish
open Elmish

type State = { Profiles: list<string> }

type Msg = | GetProfiles

let init (): State * Cmd<Msg> = { Profiles = [] }, Cmd.none

let update (msg: Msg) (state: State): State * Cmd<Msg> = state, Cmd.none


let view =
  React.functionComponent (fun () ->
    let state, dispatch = React.useElmish (init, update, [||])
    Html.article [])

module Admin

open Feliz
open Feliz.UseElmish
open Elmish

type private State = { Profiles: list<string> }

type private Msg = | GetProfiles

let private init (): State * Cmd<Msg> = { Profiles = [] }, Cmd.none

let private update (msg: Msg) (state: State): State * Cmd<Msg> = state, Cmd.none


let view =
  React.functionComponent (fun () ->
    let state, dispatch = React.useElmish (init, update, [||])
    Html.article [])


let editPlayground =
  React.functionComponent (fun () ->
    let state, dispatch = React.useElmish (init, update, [||])
    Html.article [])

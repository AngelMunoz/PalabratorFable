module App

open Fable.Core
open Feliz
open Elmish
open Feliz.Router
open Types

type Page =
  | Playground of playground: string
  | Admins

type State = { Page: Page; Error: Option<string> }

type Msg =
  | PageChanged of Page
  | Error of exn

let init () =
  {
    Page = Playground "default1"
    Error = None
  },
  Cmd.none

let update (msg: Msg) (state: State) =
  match msg with
  | PageChanged page -> { state with Page = page }, Cmd.none
  | Error exn -> { state with Error = Some exn.Message }, Cmd.none

let parseUrl: list<string> -> Page =
  function
  | []
  | [ "playgrounds" ] -> Playground "default1"
  | [ "playgrounds"; name ] -> Playground name
  | [ "admins" ] -> Admins
  | _ -> Playground "default1"


let render (state: State) (dispatch: Msg -> unit) =
  let currentPage =
    match state.Page with
    | Admins -> Admin.view ()
    | Playground name -> Playground.view { _id = name }

  React.router [
    router.onUrlChanged (parseUrl >> PageChanged >> dispatch)
    router.children currentPage
  ]

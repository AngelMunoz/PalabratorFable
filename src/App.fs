module App

open Feliz
open Elmish
open Feliz.Router

type Page =
  | Profiles
  | User of string
  | Playground of user: string * playground: string
  | Admin of string
  | EditPlayground of string

type State = { Page: Page }

type Msg = PageChanged of Page

let init () = { Page = Profiles }, Cmd.none

let update (msg: Msg) (state: State) =
  match msg with
  | PageChanged page -> { state with Page = page }, Cmd.none

let parseUrl: list<string> -> Page =
  function
  | [] -> Profiles
  | [ "user"; user ] -> User user
  | [ "user"; user; "playground"; playground ] -> Playground(user, playground)
  | [ "admin"; admin ] -> Admin admin
  | [ "playground"; playground; "edit" ] -> EditPlayground playground
  | _ -> Profiles

let render (state: State) (dispatch: Msg -> unit) =
  let currentPage =
    match state.Page with
    | Profiles -> Profiles.view ()
    | User user -> User.view ()
    | Playground (user, playground) -> Playground.view ()
    | Admin admin -> Admin.view ()
    | EditPlayground playground -> Admin.editPlayground ()

  React.router [ router.onUrlChanged (parseUrl >> PageChanged >> dispatch)
                 router.children currentPage ]

module Admin

open Browser.Types
open Fable.Core
open Feliz
open Feliz.UseElmish
open Elmish
open Types

type private State =
  {
    Admins: list<Admin>
    Error: Option<string>
  }

type private Msg =
  | GetAdmins
  | GetAdminsSuccess of list<Admin>
  | Error of exn

let private init: State * Cmd<Msg> =
  { Admins = []; Error = None }, Cmd.ofMsg GetAdmins

let private update (msg: Msg) (state: State): State * Cmd<Msg> = state, Cmd.none

type AdminFormProps =
  {
    onSubmit: {| name: string
                 pin: string
                 picture: Option<File> |} -> unit
  }

let createAdminForm =
  React.functionComponent (fun (props: AdminFormProps) ->
    let state, setState =
      React.useState
        ({|
           name = ""
           pin = ""
           picture = None
         |})

    Html.form [
      prop.onSubmit (fun e ->
        e.preventDefault ()
        props.onSubmit state)
      prop.classes [
        NesCss.NesContainer
        NesCss.WithTitle
      ]
      prop.children [
        Html.p [
          prop.className NesCss.Title
          prop.text "Add a new admin"
        ]
        Html.section [
          prop.className ""
          prop.children [
            Html.label [
              prop.text "Name"
            ]
            Html.input [
              prop.className NesCss.NesInput
              prop.required true
              prop.onTextChange (fun text ->
                setState ({| state with name = text |}))
            ]
          ]
        ]
        Html.section [
          prop.className ""
          prop.children [
            Html.label [
              prop.text "Pin"
            ]
            Html.input [
              prop.className NesCss.NesInput
              prop.type'.text
              prop.inputMode.numeric
              prop.required true
              prop.onTextChange (fun text ->
                setState ({| state with pin = text |}))
            ]
          ]
        ]
        Html.section [
          prop.className ""
          prop.children [
            Html.label [
              prop.text "Picture (optional)"
            ]
            Html.input [
              prop.className NesCss.NesInput
              prop.type'.file
              prop.accept "image/*"
              prop.multiple false
              prop.onChange (fun (file: File) ->
                setState ({| state with picture = Some file |}))
            ]
          ]
        ]
        Html.section [
          prop.className ""
          prop.children [
            Html.button [
              prop.className NesCss.NesBtn
              prop.text "Submit"
            ]
          ]
        ]
      ]
    ])

let view =
  React.functionComponent (fun () ->
    let state, dispatch = React.useElmish (init, update, [||])

    Html.article [])


let editPlayground =
  React.functionComponent (fun () -> Html.article [])

module Playground

open Fable.Core
open Fable.Core.JsInterop
open Feliz
open Feliz.UseElmish
open Elmish
open Types
open Game


[<Import("PlaygroundService", from = "./database/interop")>]
let PlaygroundService: IPlaygroundService = jsNative

type private State = { PlaygroundData: Option<Playground> }

type private Msg =
  | GetPlaygroundData of _id: Option<string>
  | GetPlaygroundDataSuccess of {| _id: string
                                   _rev: string
                                   name: string
                                   picture: string |}
  | Error of exn

let private init' (_id: string): State * Cmd<Msg> =
  { PlaygroundData = None }, Cmd.ofMsg (GetPlaygroundData(Option.ofObj _id))

let private update' (msg: Msg)
                    (state: State)
                    (playgrounds: IPlaygroundService)
                    : State * Cmd<Msg> =
  match msg with
  | GetPlaygroundData _id ->
      match _id with
      | Some _id ->
          state,
          Cmd.OfPromise.either
            playgrounds.GetPlayground
            _id
            GetPlaygroundDataSuccess
            Error
      | None ->
          state,
          Cmd.OfPromise.either
            playgrounds.GetPlayground
            "default1"
            GetPlaygroundDataSuccess
            Error
  | GetPlaygroundDataSuccess data ->
      let playground =
        Playground.Create
          {| data with
              picture = data.picture |> Option.ofObj
          |}

      { state with
          PlaygroundData = Some playground
      },
      Cmd.none
  | Error ex ->
      eprintfn "Playground UpdateError: [%s]" ex.Message
      state, Cmd.none

type PlaygroundProps = { _id: string }

let private update state msg = update' state msg PlaygroundService

let view =
  React.functionComponent (fun (props: PlaygroundProps) ->
    let state, dispatch =
      React.useElmish ((fun () -> init' props._id), update, [||])

    Html.article [
      prop.classes ["playground-area"]
      prop.children [
        PalBoard ()
      ]
    ])

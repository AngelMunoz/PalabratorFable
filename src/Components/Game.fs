module Game

open System
open Fable.Core
open Browser.Types
open Browser.Dom
open Fable.Core.JS
open Elmish
open Feliz
open Feliz.UseElmish
open Types

type ShapeProps =
  {
    id: string
    color: string
    shape: Shape
    size: Size
    position: (int * int)
    onTapped: Option<unit -> unit>
    onRequestRestroy: Option<unit -> unit>
    hidden: bool
  }

let private random = lazy (Random())

let private randomColor (colors: Option<list<string>>) =
  match colors with
  | Some colors ->
      let random = random.Value
      let index = random.Next(0, colors.Length - 1)
      colors.Item index
  | None ->
      let value = random.Value.Next(0, 4)
      match value with
      | 0 -> "red"
      | 1 -> "green"
      | 2 -> "blue"
      | _ -> "yellow"

let private randomShape (shapes: Option<list<Shape>>) =
  match shapes with
  | Some shapes ->
      let random = random.Value
      let index = random.Next(0, shapes.Length - 1)
      shapes.Item index
  | None ->
      let value = random.Value.Next(0, 4)
      match value with
      | 0 -> Circle
      | 1 -> Square
      | 2 -> Triangle
      | _ -> Triangle

let private randomSize (sizes: Option<list<Size>>) =
  match sizes with
  | Some sizes ->
      let random = random.Value
      let index = random.Next(0, sizes.Length - 1)
      sizes.Item index
  | None ->
      let value = random.Value.Next(0, 3)
      match value with
      | 0 -> Small
      | 1 -> Medium
      | _ -> Big

let private randomPoints (width, height) =
  let random = random.Value
  random.Next(0, width), random.Next(0, height)


let private getShape (props: Shape * string)
                     (size: int * int)
                     (axles: int * int)
                     =
  let height, width = size
  let x, y = axles
  let shape, color = props
  match shape with
  | Circle ->
      Html.circle [
        prop.height height
        prop.width width
        prop.cx (x / 2)
        prop.cy (y / 2)
        prop.custom ("fill", color)
      ]
  | Square ->
      Html.rect [
        prop.height height
        prop.width width
        prop.rx (x / 2)
        prop.ry (y / 2)
        prop.custom ("fill", color)
      ]
  | Triangle ->
      Html.polygon [
        prop.points [
          0, 0
          50, 50
          100, 0
        ]
        prop.custom ("fill", color)
      ]
  | Star ->
      Html.polygon [
        prop.points [
          0, 0
          50, 50
          100, 0
          50, 50
        ]
        prop.custom ("fill", color)
      ]


let private shape' (props: ShapeProps) =

  let state, setState =
    React.useState
      ({|
         color = props.color
         position = props.position
         shape = props.shape
         size = props.size
       |})

  React.useEffect (fun _ ->
    match props.onRequestRestroy with
    | Some onRequestDestroy ->
        let time = 2000

        let subId =
          setTimeout (fun _ -> onRequestDestroy ()) time

        { new IDisposable with
            member this.Dispose() = clearTimeout (subId)
        }
    | None ->
        { new IDisposable with
            member this.Dispose() = ()
        })

  let width, height =
    match state.size with
    | Small -> 50, 50
    | Medium -> 100, 100
    | Big -> 200, 200

  let px, py = state.position
  Html.svg [
    prop.onClick (fun _ ->
      props.onTapped
      |> Option.map (fun fn -> fn ())
      |> ignore)
    prop.style [
      style.height width
      style.width height
      style.position.absolute
      style.top py
      style.left px
    ]
    prop.classes [
      if props.hidden then "destroying" else "rotative"
    ]
    prop.viewBox (0, 0, 100, 100)
    prop.children [
      getShape (state.shape, state.color) (width, height) state.position
    ]
  ]

let PalShape =
  React.functionComponent ("PalShape", shape')


let generateShapeProps (props: {| amount: Option<int>
                                  shapes: Option<list<Shape>>
                                  sizes: Option<list<Size>>
                                  colors: Option<list<string>>
                                  hidden: bool
                                  windowSize: float * float
                                  onTapped: Option<unit -> unit>
                                  onRequestDestroy: Option<unit -> unit> |})
                       : list<ShapeProps> =
  let amount = defaultArg props.amount 10
  [
    for i in 0 .. amount do
      let width, height = props.windowSize
      {
        id = sprintf "%i:%i" i DateTime.Now.Millisecond
        onTapped = props.onTapped
        onRequestRestroy = props.onRequestDestroy
        color = randomColor props.colors
        shape = randomShape props.shapes
        size = randomSize props.sizes
        position = randomPoints (width |> int, height |> int)
        hidden = props.hidden
      }
  ]


type State =
  {
    Items: list<ShapeProps>
    WindowSize: float * float
  }

type Msg =
  | SetWindowSize of float * float
  | GenerateProps
  | GenerateSingle
  | GenerateDelayed of int
  | UpdateItem of item: ShapeProps
  | RemoveItem of item: ShapeProps

let init () =
  let width = window.innerWidth - 200.
  let height = window.innerHeight - 200.
  {
    Items = []
    WindowSize = width, height
  },
  Cmd.ofMsg GenerateProps

let update (msg: Msg) (state: State) =
  match msg with
  | SetWindowSize (width, height) ->
      { state with
          WindowSize = width, height
      },
      Cmd.none
  | GenerateProps ->
      let items =
        generateShapeProps
          {|
            amount = None
            shapes = None
            sizes = None
            colors = None
            hidden = false
            onTapped = None
            windowSize = state.WindowSize
            onRequestDestroy = None
          |}

      { state with Items = items }, Cmd.none
  | UpdateItem item ->
      let items =
        state.Items
        |> List.map (fun i ->
             if i.id = item.id then { item with hidden = true } else i)

      let removeDelayed () =
        async {
          do! Async.Sleep 1500
          return item
        }

      { state with Items = items },
      Cmd.OfAsync.perform removeDelayed () RemoveItem
  | RemoveItem item ->
      let items =
        state.Items
        |> List.filter (fun i -> i.id <> item.id)

      let time = random.Value.Next(1000, 2501)
      { state with Items = items }, Cmd.ofMsg (GenerateDelayed time)
  | GenerateDelayed time ->
      let delay () = async { do! Async.Sleep 2500 }
      state, Cmd.OfAsync.perform delay () (fun _ -> GenerateSingle)
  | GenerateSingle ->
      let items =
        match state.Items.Length > 10 with
        | false ->
            let prop =
              generateShapeProps
                {|
                  amount = Some 1
                  shapes = None
                  sizes = None
                  colors = None
                  hidden = false
                  onTapped = None
                  windowSize = state.WindowSize
                  onRequestDestroy = None
                |}

            let items = [ state.Items; prop ] |> List.concat
            items
        | true ->
            let length =
              if state.Items.Length <= 10 then state.Items.Length else 9

            state.Items.[..length]

      { state with Items = items }, Cmd.none


let private board' () =
  let state, dispatch = React.useElmish (init, update, [||])

  React.useEffect (fun _ ->
    let updateSize _ =
      (window.innerWidth - 200., window.innerHeight - 200.)
      |> SetWindowSize
      |> dispatch

    window.addEventListener ("resize", updateSize)
    { new IDisposable with
        member _.Dispose() =
          window.removeEventListener ("resize", updateSize)
    })
  let removeNow props = props |> RemoveItem |> dispatch
  let removeWithDelay (props: ShapeProps) = props |> UpdateItem |> dispatch
  Html.div [
    prop.className "pal-board"
    prop.children [
      for props in state.Items do
        PalShape
          { props with
              onTapped = Some(fun () -> removeWithDelay props)
              onRequestRestroy = Some(fun () -> removeWithDelay props)
              position =
                randomPoints
                  (window.innerWidth |> int, window.innerHeight |> int)
          }
    ]

  ]

let PalBoard =
  React.functionComponent ("PalBoard", board')

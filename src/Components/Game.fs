module Game

open System
open Feliz
open Types
open Fable.Core.JS

type ShapeProps =
  {
    color: string
    shape: Shape
    size: Size
    position: (int * int)
    zIndex: int
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
      | _ -> Star

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

let private randomPoints () =
  let random = random.Value
  random.Next(0, 500), random.Next(0, 500)


let private shape' (props: ShapeProps) =
  let x, y =
    match props.size with
    | Small -> 50, 50
    | Medium -> 100, 100
    | Big -> 200, 200

  let px, py = props.position
  Html.svg [
    prop.style [
      style.height x
      style.width y
      style.position.absolute
      style.top py
      style.left px
      style.zIndex props.zIndex
    ]
    prop.viewBox (0, 0, 100, 100)
    prop.children [
      match props.shape with
      | Circle ->
          Html.circle [
            prop.height y
            prop.width x
            prop.cx (x / 2)
            prop.cy (y / 2)
            prop.custom ("fill", props.color)
          ]
      | Square ->
          Html.rect [
            prop.height y
            prop.width x
            prop.rx (x / 2)
            prop.ry (y / 2)
            prop.custom ("fill", props.color)
          ]
      | Triangle ->
          Html.polygon [
            prop.points [
              0, 0
              50, 50
              100, 0
            ]
            prop.custom ("fill", props.color)
          ]
      | Star ->
          Html.polygon [
            prop.points [
              0, 0
              50, 50
              100, 0
              50, 50
            ]
            prop.custom ("fill", props.color)
          ]
    ]
  ]

let PalShape =
  React.functionComponent ("PahShape", shape')

type BoardProps =
  | GenerateProps of {| amount: Option<int>
                        shapes: Option<list<Shape>>
                        sizes: Option<list<Size>>
                        colors: Option<list<string>> |}
  | GeneratedProps of list<ShapeProps>

let generateShapeProps (props: {| amount: Option<int>
                                  shapes: Option<list<Shape>>
                                  sizes: Option<list<Size>>
                                  colors: Option<list<string>> |})
                       : list<ShapeProps> =
  let amount = defaultArg props.amount 10
  [
    for i in 0 .. amount do
      {
        color = randomColor props.colors
        shape = randomShape props.shapes
        size = randomSize props.sizes
        position = randomPoints ()
        zIndex = i
      }
  ]

let private board' (props: BoardProps) =
  let state, setState = React.useState (props)

  React.useEffect (fun () ->
    let timerid =
      setInterval (fun () ->
        let props =
          match state with
          | GenerateProps props -> GeneratedProps(generateShapeProps props)
          | GeneratedProps props ->
              props
              |> List.map (fun prop ->
                   { prop with
                       position = randomPoints ()
                       color = randomColor None
                       size = randomSize None
                       shape = randomShape None
                   })
              |> GeneratedProps

        printfn "interval"
        setState props) 1000

    { new IDisposable with
        member this.Dispose() = clearInterval (timerid)
    })

  React.fragment [
    match state with
    | GeneratedProps state ->
        for prop in state do
          PalShape prop
    | GenerateProps props -> Html.none
  ]

let PalBoard =
  React.functionComponent ("PalBoard", board')

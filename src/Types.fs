module Types

open Fable.Core
open Zanaptak.TypedCssClasses


type Size =
  | Small
  | Medium
  | Big


type Shape =
  | Circle
  | Square
  | Triangle
  | Star

type Picture =
  | File of string
  | Shape of Shape

  static member FromString(picture: string) =
    match picture.ToLowerInvariant() with
    | "circle" -> Shape Circle
    | "square" -> Shape Square
    | "triangle" -> Shape Triangle
    | "star" -> Shape Star
    | _ -> File picture

type Admin =
  {
    _id: string
    _rev: string
    name: string
    pin: string
    picture: string
  }

type Playground =
  {
    _id: string
    _rev: string
    name: string
    picture: Picture
  }

  static member Create(partial: {| _id: string
                                   _rev: string
                                   name: string
                                   picture: Option<string> |})
                      =
    {
      _id = partial._id
      _rev = partial._rev
      name = partial.name
      picture =
        (Picture.FromString(partial.picture |> Option.defaultValue "circle"))
    }


type NesCss =
  CssClasses<"../node_modules/nes.css/css/nes-core.css", Naming.PascalCase>

type IPlaygroundService =
  abstract GetPlayground: string
   -> JS.Promise<{| _id: string
                    _rev: string
                    name: string
                    picture: string |}>

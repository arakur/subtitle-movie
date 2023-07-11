namespace Color

open FSharp.Data

type IColor =
    abstract member Rgba: unit -> byte * byte * byte * byte

module IColor =
    let Rgba (c: IColor) = c.Rgba()

    let R (c: IColor) =
        let r, _, _, _ = c.Rgba()
        r

    let G (c: IColor) =
        let _, g, _, _ = c.Rgba()
        g

    let B (c: IColor) =
        let _, _, b, _ = c.Rgba()
        b

    let A (c: IColor) =
        let _, _, _, a = c.Rgba()
        a

    let Rgb (c: IColor) =
        let r, g, b, _ = c.Rgba()
        r, g, b

    let RgbaHexHash (c: IColor) =
        let r, g, b, a = c.Rgba()
        $"#{r:x2}{g:x2}{b:x2}{a:x2}"

    let RgbaHex0x (c: IColor) =
        let r, g, b, a = c.Rgba()
        $"0x{r:x2}{g:x2}{b:x2}{a:x2}"

    let RgbHexHash (c: IColor) =
        let r, g, b = Rgb c
        $"#{r:x2}{g:x2}{b:x2}"

    let RgbHex0x (c: IColor) =
        let r, g, b = Rgb c
        $"0x{r:x2}{g:x2}{b:x2}"

type Rgba =
    { R: byte
      G: byte
      B: byte
      A: byte }

    interface IColor with
        member this.Rgba() = this.R, this.G, this.B, this.A

type Rgb =
    { R: byte
      G: byte
      B: byte }

    member this.AsRgba() =
        { R = this.R
          G = this.G
          B = this.B
          A = 255uy }

    interface IColor with
        member this.Rgba() = (this.AsRgba(): IColor).Rgba()

module NamedColor =
    open System

    type NamedColorProvider = JsonProvider<"../../color-names.jsonc">

    let TryGetColorCode (name: string) : Rgb option =
        NamedColorProvider.GetSample().JsonValue.TryGetProperty name
        |> Option.map (fun value ->
            let s = value.AsString()
            let r = Convert.ToByte(s.Substring(1, 2), 16)
            let g = Convert.ToByte(s.Substring(3, 2), 16)
            let b = Convert.ToByte(s.Substring(5, 2), 16)
            { R = r; G = g; B = b })

type NamedColor(name: string, color: IColor) =
    static member TryNew(name: string) =
        NamedColor.TryGetColorCode name |> Option.map (fun rgb -> NamedColor(name, rgb))

    interface IColor with
        member __.Rgba() = color.Rgba()

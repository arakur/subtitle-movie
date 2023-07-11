namespace Subtitle

open Font
open Color
open Measure

type TypstBuilder =
    { Width: Length option
      Height: Length option
      Size: Length option
      Fill: IColor option
      Fonts: FamilyList option
      Weight: Weight option
      Style: Style option
      RestPageOptions: Map<string, string>
      RestTextOptions: Map<string, string> }

    member private this.PageOptionList() =
        seq {
            match this.Width with
            | Some width -> yield "width", width.ToString()
            | None -> ()

            match this.Height with
            | Some height -> yield "height", height.ToString()
            | None -> ()

            for kv in this.RestPageOptions do
                yield kv.Key, kv.Value
        }
        |> Seq.toList

    member private this.TextOptionList() =
        seq {
            match this.Size with
            | Some size -> yield "size", size.ToString()
            | None -> ()

            match this.Fill with
            | Some fill -> yield "fill", $"rgb({fill |> IColor.RgbaHexHash})"
            | None -> ()

            match this.Fonts with
            | Some fonts -> yield "font", fonts.ToString()
            | None -> ()

            match this.Weight with
            | Some weight -> yield "weight", weight.ToString()
            | None -> ()

            match this.Style with
            | Some style -> yield "style", style.ToString()
            | None -> ()

            for kv in this.RestTextOptions do
                yield kv.Key, kv.Value
        }
        |> Seq.toList

    member this.Build text =
        let pageOptions =
            this.PageOptionList()
            |> List.map (fun (key, value) -> sprintf "%s: %s" key value)
            |> String.concat ", "

        let textOptions =
            this.TextOptionList()
            |> List.map (fun (key, value) -> sprintf "%s: %s" key value)
            |> String.concat ", "

        $"#set page({pageOptions})\n#set text({textOptions})\n{text}"

module TypstBuilder =
    let Default: TypstBuilder =
        { Width = None
          Height = None
          Size = None
          Fill = None
          Fonts = None
          Weight = None
          Style = None
          RestPageOptions = Map.empty
          RestTextOptions = Map.empty }

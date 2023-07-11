module Ffmpeg.Input

type IInputPattern =
    abstract member ComposeInput: string

type FileInput(Path: Path, Format: string option) =

    interface IInputPattern with
        member __.ComposeInput: string =
            let format = Format |> Option.map (fun s -> $@"-f {s}") |> Option.defaultValue ""

            $@"{format} -i ""{Path}"""

type BgInput(BgColor: string, Width: int, Height: int) =

    interface IInputPattern with
        member __.ComposeInput: string =
            $@"-f lavfi -i ""color=c={BgColor}:s={Width}x{Height}"""

type FilterInput(Filter: string, Format: string option) =

    interface IInputPattern with
        member __.ComposeInput: string =
            let format = Format |> Option.map (fun s -> $@"-f {s}") |> Option.defaultValue ""

            $@"{format} -i ""{Filter}"""

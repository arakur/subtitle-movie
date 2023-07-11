module Ffmpeg.FilterNode

type FilterTerm = string

// a or v
type FilterNodeKind =
    | Audio
    | Video
    | AudioVideo

    member this.Compose() =
        match this with
        | Audio -> ":a"
        | Video -> ":v"
        | AudioVideo -> ""

type FilterNode =
    | Input of int * FilterNodeKind
    | Intermediate of int * FilterNodeKind
    | Output

    member this.Compose() =
        match this with
        | Input(input, kind) -> $@"[{input}{kind.Compose()}]"
        | Intermediate(intermediate, kind) -> $@"[m{intermediate}{kind.Compose()}]"
        | Output -> ""

module Subtitle.Font

type Family = string

type Style =
    | Normal
    | Italic
    | Oblique

    override this.ToString() =
        match this with
        | Normal -> "\"normal\""
        | Italic -> "\"italic\""
        | Oblique -> "\"oblique\""

type Weight =
    | Int of int
    | Thin
    | Extralight
    | Light
    | Regular
    | Medium
    | Semibold
    | Bold
    | Extrabold
    | Black

    override this.ToString() =
        match this with
        | Int i -> i.ToString()
        | Thin -> "\"thin\""
        | Extralight -> "\"extralight\""
        | Light -> "\"light\""
        | Regular -> "\"regular\""
        | Medium -> "\"medium\""
        | Semibold -> "\"semibold\""
        | Bold -> "\"bold\""
        | Extrabold -> "\"extrabold\""
        | Black -> "\"black\""

type FamilyList =
    { families: Family list }

    override this.ToString() =
        let interior =
            this.families |> List.map (fun f -> "\"" + f + "\"") |> String.concat ", "

        $"({interior})"

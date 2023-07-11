namespace Measure

[<Measure>]
type pt

[<Measure>]
type px

type Length =
    | Pt of float<pt>

    member this.toPt() =
        match this with
        | Pt pt -> pt

    override this.ToString() =
        match this with
        | Pt pt -> pt.ToString() + "pt"

namespace Ffmpeg

type Path =
    | Path of string

    override this.ToString() =
        match this with
        | Path path -> path

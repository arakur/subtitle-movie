namespace Ffmpeg

type VideoCodec =
    | Libx264

    override this.ToString() =
        match this with
        | Libx264 -> "-c:v libx264"

type AudioCodec =
    | Aac

    override this.ToString() =
        match this with
        | Aac -> "-c:a aac"

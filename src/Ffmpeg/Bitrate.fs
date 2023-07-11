namespace Ffmpeg

type Bitrate =
    | Kbps of int

    override this.ToString() =
        match this with
        | Kbps kbps -> $@"-b:a {kbps}k"
namespace Voicevox

type Wav =
    { data: byte[] }

    member this.WriteFile(path: string) : Result<unit, exn> =
        try
            System.IO.File.WriteAllBytes(path, this.data)
            Ok()
        with ex ->
            Error(ex)

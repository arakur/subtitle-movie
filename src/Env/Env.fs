namespace Env

open VoicevoxCore

type Env() =
    let mutable voicevoxCore: VoicevoxCore option = None

    interface System.IDisposable with
        member __.Dispose() =
            voicevoxCore |> Option.iter (fun core -> core.Dispose())

    member __.LaunchVoicevoxCore() : VoicevoxCore =
        match voicevoxCore with
        | Some core -> core
        | None ->
            let core = new VoicevoxCore()
            voicevoxCore <- Some(core)
            core

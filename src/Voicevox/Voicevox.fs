namespace Voicevox

open VoicevoxCore
open Synthesizer
open Speaker

type Synthesizer(core: VoicevoxCore, id: VoiceId) =
    interface ISynthesizer with
        member __.Synthesize(text: string) : Wav option =
            let struct (data, success) = core.Synthesize(text, id)
            if success then Some({ data = data }) else None

type Speaker(id: VoiceId) =
    interface ISpeaker with
        member __.Synthesizer env =
            let core = env.LaunchVoicevoxCore()
            Synthesizer(core, id)

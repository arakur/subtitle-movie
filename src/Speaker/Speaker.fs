namespace Speaker

open Env
open Synthesizer

type ISpeaker =
    abstract member Synthesizer: Env -> ISynthesizer

type VoidSpeaker() =
    interface ISpeaker with
        member this.Synthesizer(env: Env) =
            { new ISynthesizer with
                // TODO: Wait for some seconds while speaking.
                member this.Synthesize(text: string) = Some { data = Array.zeroCreate 0 } }

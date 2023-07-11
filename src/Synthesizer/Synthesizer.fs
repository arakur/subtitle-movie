namespace Synthesizer

type ISynthesizer =
    abstract member Synthesize: string -> Wav option

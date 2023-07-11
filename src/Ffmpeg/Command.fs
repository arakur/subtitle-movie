namespace Ffmpeg

open Command

open System.Diagnostics

open Input

type FfmpegCommand =
    { inputs: IInputPattern list
      output: Path
      filterComplex: FilterComplex
      videoCodec: VideoCodec option
      audioCodec: AudioCodec option
      audioBitrate: Bitrate option
      shortest: bool }

    static member Default(output: Path) =
        { inputs = []
          output = output
          filterComplex = FilterComplex()
          videoCodec = None
          audioCodec = None
          audioBitrate = None
          shortest = false }

    member this.Input(input: IInputPattern) =
        // REMARK: the input is added reversely.
        { this with
            inputs = input :: this.inputs }

    member this.Inputs(inputs: IInputPattern seq) =
        inputs |> Seq.fold (fun (acc: FfmpegCommand) -> acc.Input) this

    member this.Output(output: Path) = { this with output = output }

    member this.FilterComplex(filterComplex: FilterComplex) =
        { this with
            filterComplex = filterComplex }

    member this.VideoCodec(videoCodec: VideoCodec) =
        { this with
            videoCodec = Some videoCodec }

    member this.AudioCodec(audioCodec: AudioCodec) =
        { this with
            audioCodec = Some audioCodec }

    member this.AudioBitrate(bitrate: Bitrate) =
        { this with
            audioBitrate = Some bitrate }

    member this.Shortest() = { this with shortest = true }

    member private this.Arguments() =
        let y = "-y" // Must be given to overwrite output file.

        // REMARK: the inputs are concatenated reversely.
        let inputs =
            this.inputs
            |> List.map (fun input -> input.ComposeInput)
            |> List.fold (fun acc input -> $@"{input} {acc}") ""

        let filter = this.filterComplex.Compose()

        let videoCodec =
            this.videoCodec
            |> Option.map (fun videoCodec -> videoCodec.ToString())
            |> Option.defaultValue ""

        let audioCodec =
            this.audioCodec
            |> Option.map (fun audioCodec -> audioCodec.ToString())
            |> Option.defaultValue ""

        let audioBitrate =
            this.audioBitrate
            |> Option.map (fun audioBitrate -> audioBitrate.ToString())
            |> Option.defaultValue ""

        let shortest = if this.shortest then "-shortest" else ""

        let output = this.output

        $@"{y} {inputs} {filter} {videoCodec} {audioCodec} {audioBitrate} {shortest} ""{output}"""

    member this.Start() : FfmpegProcess =
        ProcessStartInfo(
            FileName = Exe.ffmpeg.ToString(),
            Arguments = this.Arguments(),
            CreateNoWindow = true,
            RedirectStandardError = true
        )
        |> Process.Start
        |> FfmpegProcess

    override this.ToString() =
        let arguments = this.Arguments()
        $@"{Exe.ffmpeg} {arguments}"

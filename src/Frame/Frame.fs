namespace Frame

open Measure
open Color
open Speaker
open Subtitle
open Env
open Ffmpeg
open System.Collections.Immutable

type queue<'T> = ImmutableQueue<'T>

//

type Dialogue =
    { Text: string
      Speak: string }

    static member Plain text = { Text = text; Speak = text }



type SpeakerSetting =
    { Speaker: ISpeaker
      TypstBuilder: TypstBuilder }

    static member Default =
        { Speaker = VoidSpeaker()
          TypstBuilder = TypstBuilder.Default }

type FrameOption =
    { SpeakerSetting: SpeakerSetting
      X: float<px>
      Y: float<px>
      BgColor: IColor }

    static member Default =
        { SpeakerSetting = SpeakerSetting.Default
          X = 0.<px>
          Y = 0.<px>
          BgColor = { R = 0uy; G = 0uy; B = 0uy; A = 0uy } }

type Frame =
    { Dialogue: Dialogue
      Option: FrameOption }

    member this.Render (env: Env) (subtitlePath: string) (voicePath: string) (videoPath: string) =
        // Render subtitle.
        Subtitle.Render this.Option.SpeakerSetting.TypstBuilder subtitlePath this.Dialogue.Text

        // Render voice.
        let synthesizer = this.Option.SpeakerSetting.Speaker.Synthesizer env
        let wav = synthesizer.Synthesize this.Dialogue.Speak
        let res = wav |> Option.map (Synthesizer.Wav.writeFile voicePath)

        // Marge subtitle and voice.
        let filterComplex =
            FilterComplex()
                .Add(
                    [ FilterNode.Input(0, FilterNode.AudioVideo)
                      FilterNode.Input(1, FilterNode.AudioVideo) ],
                    [ FilterNode.Output ],
                    Filter.Overlay($"{this.Option.X}", $"{this.Option.Y}")
                )

        let command =
            FfmpegCommand
                .Default(Path videoPath)
                .Input(Input.BgInput(this.Option.BgColor |> IColor.RgbaHex0x, 1280, 720))
                .Input(Input.FileInput(Path subtitlePath, None))
                .Input(Input.FileInput(Path voicePath, None))
                .FilterComplex(filterComplex)
                .VideoCodec(Libx264)
                .AudioCodec(Aac)
                .AudioBitrate(Kbps 128)
                .Shortest()

        // DEBUG
        printfn "Executing: %s" <| command.ToString()

        let proc = command.Start()
        proc.StandardError.ReadToEnd() |> ignore // TODO: Handle the log suitably.
        proc.WaitForExit()

        // TODO: Handle the error.
        match res with
        | Some(Error e) -> raise e
        | _ -> ()

type FrameList =
    { Frames: Frame queue
      Length: int }

    static member Empty =
        { Frames = ImmutableQueue<Frame>.Empty
          Length = 0 }

    member this.Add(frame: Frame) =
        { this with
            Frames = this.Frames.Enqueue frame
            Length = this.Length + 1 }

    member this.Render
        (env: Env)
        (subtitlePath: int -> string)
        (voicePath: int -> string)
        (videoPath: int -> string)
        (outPath: string)
        =
        for i, frame in this.Frames |> Seq.indexed do
            frame.Render env (subtitlePath i) (voicePath i) (videoPath i)

        // Concat frames.

        let outFilterComplex =
            FilterComplex()
                .Add(
                    seq {
                        for i in 0 .. this.Length - 1 do
                            yield FilterNode.Input(i, FilterNode.Video)
                            yield FilterNode.Input(i, FilterNode.Audio)
                    }
                    |> Seq.toList,
                    [ FilterNode.Output ],
                    Filter.Concat(num = this.Length)
                )

        let outCommand =
            FfmpegCommand
                .Default(output = Path outPath)
                .Inputs(
                    seq {
                        for i in 0 .. this.Length - 1 do
                            yield Input.FileInput(Path <| videoPath i, None)
                    }
                )
                .FilterComplex(outFilterComplex)
                .VideoCodec(Libx264)
                .AudioCodec(Aac)
                .AudioBitrate(Kbps 128)
                .Shortest()

        // DEBUG
        printfn "Executing: %s" <| outCommand.ToString()

        let proc = outCommand.Start()

        proc.StandardError.ReadToEnd() |> ignore // TODO: Handle the log suitably.

        proc.WaitForExit()

type FrameListBuilder(FrameList: FrameList, Option: FrameOption) =
    new() =
        FrameListBuilder(
            FrameList.Empty,
            { SpeakerSetting =
                { Speaker = Speaker.VoidSpeaker()
                  TypstBuilder = TypstBuilder.Default }
              BgColor = { R = 0uy; G = 0uy; B = 0uy; A = 255uy } // The initial color is black.
              X = 0.<px> // The initial position is (0, 0).
              Y = 0.<px> }
        )

    member __.Add(dialogue: Dialogue) =
        FrameListBuilder(FrameList.Add { Dialogue = dialogue; Option = Option }, Option)

    member this.Add(plain: string) = this.Add(Dialogue.Plain plain)

    member this.Add(text: string, speak: string) =
        this.Add({ Text = text; Speak = speak })

    member __.Modify(f: FrameOption -> FrameOption) = FrameListBuilder(FrameList, f Option)

    member this.SetOption(option: FrameOption) = this.Modify(fun _ -> option)

    member this.SetSpeaker(speakerSetting: SpeakerSetting) =
        this.Modify(fun option ->
            { option with
                SpeakerSetting = speakerSetting })

    member __.Build() = FrameList

open Measure
open Color
open Subtitle
open Env
open Frame

//

let outputDir = "../../output"

let tmpPath i ext = $"{outputDir}/{i}.{ext}"

let outPath = $"{outputDir}/out.mp4"

//

let initTypstBuilder =
    { TypstBuilder.Default with
        Width = Some <| Pt 300.<pt>
        Height = Some <| Pt 100.<pt>
        Size = Some <| Pt 20.<pt>
        Weight = Some Font.Bold
        Fonts = Some { families = [ "Constan"; "Yu Mincho" ] } }

let initFrameOption =
    { FrameOption.Default with
        BgColor =
            { R = 255uy
              G = 255uy
              B = 255uy
              A = 255uy }
        X = 0.<px>
        Y = 0.<px> }

let 春日部つむぎ =
    { Speaker = Voicevox.Speaker(8)
      TypstBuilder =
        { initTypstBuilder with
            Fill = Some <| { R = 186uy; G = 167uy; B = 204uy } } }

let 四国めたん =
    { Speaker = Voicevox.Speaker(2)
      TypstBuilder =
        { initTypstBuilder with
            Fill = Some <| { R = 231uy; G = 97uy; B = 164uy } } }

let frameList: FrameList =
    FrameListBuilder()
        .SetOption(initFrameOption)
        .SetSpeaker(春日部つむぎ)
        .Add(text = "モナドは単なる自己関手の圏におけるモノイド対象だよ．", speak = "モナドは単なる自己かん手の圏におけるモノイド対象だよ．")
        .Add(plain = "何か問題でも？")
        .SetSpeaker(四国めたん)
        .Add(plain = "おっそうだな")
        .Build()

// TODO: Make the rendering process concurrent.

do
    use env = new Env()
    frameList.Render env (fun i -> tmpPath i "png") (fun i -> tmpPath i "wav") (fun i -> tmpPath i "mp4") outPath

namespace Command

open FSharp.Data

module Command =
    // REMARK: This is the location of the json file which specifies the places of the executables.
    [<Literal>]
    let CommandConfigPath = "../../commandConfig.json"

    type CommandConfig = JsonProvider<CommandConfigPath>

    let commands = CommandConfig.GetSample()

open Command

module Exe =
    open System.IO
    // If the path is relative, it is regarded as relative to the location of `CommandConfigPath` and converted to an absolute path.
    let private addRelativePath (path: string) =
        if Path.IsPathRooted path then
            path
        else
            let dir = Path.GetDirectoryName CommandConfigPath
            Path.Combine(dir, path)

    let typst = commands.Typst |> addRelativePath
    let magick = commands.Magick |> addRelativePath
    let ffmpeg = commands.Ffmpeg |> addRelativePath

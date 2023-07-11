namespace Ffmpeg

open System.Diagnostics

type FfmpegProcess(proc: Process) =
    member __.StandardError = proc.StandardError

    member __.WaitForExit() = proc.WaitForExit()

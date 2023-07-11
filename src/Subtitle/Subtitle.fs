namespace Subtitle

open System.IO
open System.Diagnostics
open System

open Measure
open Command

module Subtitle =
    // REMARK:
    //  We make width of A4 paper coincide with 1280 px, which is HD resolution.
    //  That is: 8.27 inch = 595.44 pt <-> 1280 px
    let dotsParPt = 1280.<px> / 595.44<pt>

    let Render (typstBuilder: TypstBuilder) (outputPath: string) (text: string) =
        // TODO: Make it configurable.
        let density = 1000

        let geometry = typstBuilder.Width

        // TODO: Write a module for temporary files.
        let tempPath0 = Path.GetTempPath() + Guid.NewGuid().ToString() + ".typ"

        let tempPath1 = Path.GetTempPath() + Guid.NewGuid().ToString() + ".pdf"

        let typ = typstBuilder.Build text

        let typstProc =
            ProcessStartInfo(
                FileName = Exe.typst,
                Arguments = $"compile \"{tempPath0}\" \"{tempPath1}\"",
                CreateNoWindow = true,
                RedirectStandardError = true
            )

        let imageMagickProc =
            let geometry =
                geometry
                |> Option.map (fun geometry ->
                    let geometryPx: float<px> = geometry.toPt () * dotsParPt
                    $"-geometry {geometryPx}")
                |> Option.defaultValue ""

            ProcessStartInfo(
                FileName = Exe.magick,
                Arguments = $"convert -density {density} {geometry} \"{tempPath1}\" \"{outputPath}\"",
                CreateNoWindow = true,
                RedirectStandardError = true
            )

        // TODO: Catch exceptions.

        File.WriteAllText(tempPath0, typ)

        // TODO: Check if the result pdf file has only one page, that is, the text is rendered in the specified width and height.

        Process.Start(typstProc).WaitForExit()

        Process.Start(imageMagickProc).WaitForExit()

        File.Delete(tempPath0)

        File.Delete(tempPath1)

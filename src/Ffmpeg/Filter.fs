namespace Ffmpeg

open FilterNode

type Filter =
    interface
        abstract member Compose: unit -> string
    end

module Filter =

    type Scale(width: FilterTerm, height: FilterTerm) =
        interface Filter with
            member __.Compose() = $@"scale={width}:{height}"

    type Split(num: int) =
        interface Filter with
            member __.Compose() = $@"split={num}"

    type Overlay(x: FilterTerm, y: FilterTerm) =
        interface Filter with
            member __.Compose() = $@"overlay={x}:{y}"

    type Concat(num: int) =
        interface Filter with
            member __.Compose() = $@"concat=n={num}:v=1:a=1"

    type Other(raw: string) =
        interface Filter with
            member __.Compose() = raw

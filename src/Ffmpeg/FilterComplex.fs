namespace Ffmpeg

open FilterNode

type FilterEdge =
    { inputs: FilterNode list
      outputs: FilterNode list
      filter: Filter }

    member this.Compose() =
        let inputs =
            this.inputs |> List.map (fun input -> input.Compose()) |> String.concat ""

        let outputs =
            this.outputs |> List.map (fun output -> output.Compose()) |> String.concat ""

        let filter = this.filter.Compose()

        $@"{inputs} {filter} {outputs}"

// filters are stored in reverse order
type FilterComplex(filters: FilterEdge list) =
    new() = FilterComplex([])

    member private _.Add(filter: FilterEdge) = FilterComplex(filter :: filters)

    member this.Add(inputs: FilterNode list, outputs: FilterNode list, filter: Filter) =
        this.Add(
            { inputs = inputs
              outputs = outputs
              filter = filter }
        )

    member __.Compose() =
        // It concatenates filters in reverse order.
        filters
        |> List.map (fun filter -> filter.Compose())
        |> List.fold (fun acc filter -> $@"{filter};{acc}") ""
        |> fun s -> $@"-filter_complex ""{s}"""

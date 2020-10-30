namespace DistanceMonitoring.Mock

open System
open DistanceMonitoring.Data

module Instance =
    let stream origins guid = seq { 
        while true do 
            yield async { 
                do! Async.Sleep 1000
                return { Guid = guid
                         DetectorDistances = origins 
                                        |> List.map ^ fun o -> 
                                            {| origin = o; distance = 0.0 |} } }
    }

    let setupStream () = stream [] ^ Guid.NewGuid ()

module Say =
    let hello name =
        printfn "Hello %s" name

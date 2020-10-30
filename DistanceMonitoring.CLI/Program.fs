namespace DistanceMonitoring.CLI

open System
open DistanceMonitoring
open DistanceMonitoring.Data

module Main =
    [<EntryPoint>]
    let main argv =
        for item in Mock.Instance.setupStream () do
            async { 
                let! item = item 
                printfn "%A" (item |> Serializer.serializeTo Json)
                printfn "%A" (item |> Serializer.serializeTo Xml) 
            }
            |> Async.RunSynchronously
        0
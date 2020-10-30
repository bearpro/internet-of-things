namespace DistanceMonitoring.CLI

open System
open DistanceMonitoring
open DistanceMonitoring.Data

module Main =
    [<EntryPoint>]
    let main argv =
        for asyncItem in Mock.Instance.setupStream () do
            async { 
                let! item = asyncItem 
                let serializedJson = item |> Serializer.serializeTo Json
                let serializedXml = item |> Serializer.serializeTo Xml
                let deserialzedJson = serializedJson |> Serializer.deserializeFrom Json
                let deserialzedXml = serializedXml |> Serializer.deserializeFrom Xml
                printfn "[*] Payload:\n%s\n%s\n[*] Deserialized:\n%A\n%A"
                    serializedJson
                    serializedXml
                    deserialzedJson
                    deserialzedXml
            }
            |> Async.RunSynchronously
        0
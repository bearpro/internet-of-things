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
                let [json; xml; html] = [Json; Xml; Html] 
                                        |> List.map ^ fun x -> Serializer.serializeTo x item
                let serializedJson = item |> Serializer.serializeTo Json
                let serializedXml = item |> Serializer.serializeTo Xml
                let [jsonData; xmlData; htmlData] = 
                    List.map2 
                        (fun format payload -> Serializer.deserializeFrom format payload)
                        [Json; Xml; Html] 
                        [json; xml; html]
                printfn "[*] Payload:\n%s\n%s\n%s\n[*] Deserialized:\n%A\n%A\n%A\n"
                    json xml html 
                    jsonData xmlData htmlData
            }
            |> Async.RunSynchronously
        0
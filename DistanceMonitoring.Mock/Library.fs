namespace DistanceMonitoring.Mock

open System
open DistanceMonitoring.Data
open DistanceMonitoring.Utils

module Instance =

    let random = Random()
    
    let initPositions labels = 
        labels 
        |> List.map ^ fun label -> label, { X = 0.; Y = 0.}

    let updatePosition (tag, pos) =
        tag, {pos with X = pos.X + (float (random.Next(0, 100))) * 0.01
                       Y = pos.Y + (float (random.Next(0, 100))) * 0.01 }

    let distanceToOrigins tagPosition origins = 
        [ for originPosition in origins ->
            let x' = originPosition.X - tagPosition.X |> abs
            let y' = originPosition.Y - tagPosition.Y |> abs
            let r = sqrt (x' ** 2.0 + y' ** 2.0)
            { Origin = originPosition; Distance = r } ]
    
    let setupStream labels origins: seq<Async<TagData>> = 
        seq { 
            while true do 
                let mutable tags = initPositions labels
                tags <- List.map updatePosition tags
                for label, pos in tags do
                    yield async { 
                        do! Async.Sleep 1000
                        return { Label = label; DetectorDistances = (distanceToOrigins pos origins) } }
        }

namespace DistanceMonitoring.Mock

open System
open DistanceMonitoring.Data
open DistanceMonitoring.Utils

module Instance =

    let random = Random()
    
    let initPositions labels = 
        let rnd = fun () -> float <| random.Next(0, 3)
        labels 
        |> List.map ^ fun label -> label, { X = rnd(); Y = rnd()}

    let clamp value =
        if value > 3.0 then 3.0
        elif value < 0.0 then 0.0
        else value

    let updatePosition (tag, pos) =
        tag, { pos with X = clamp <| pos.X + (float (random.Next(-100, 100))) * 0.0001
                        Y = clamp <| pos.Y + (float (random.Next(-100, 100))) * 0.0001 }

    let distanceToOrigins tagPosition origins = 
        [ for originPosition in origins ->
            let x' = originPosition.X - tagPosition.X |> abs
            let y' = originPosition.Y - tagPosition.Y |> abs
            let r = sqrt (x' ** 2.0 + y' ** 2.0)
            { Origin = originPosition; Distance = r } ]
    
    let setupStream labels origins: seq<Async<TagData>> = 
        seq { 
            let mutable tags = initPositions labels
            while true do 
                tags <- List.map updatePosition tags
                for label, pos in tags do
                    yield async { 
                        do! Async.Sleep 50
                        return { Label = label; DetectorDistances = (distanceToOrigins pos origins) } }
        }

namespace DistanceMonitoring.Web.Services

open System
open System.Text
open DistanceMonitoring.Data.CommonTypes
open DistanceMonitoring.Utils


module PositionCalculator =
    let intersectionPoints (positionA, radiusA) (positionB, radiusB) =
        let x0, x1, y0, y1, r0, r1 =
            positionA.X, positionB.X, positionA.Y, positionB.Y, radiusA, radiusB

        let d =
            Math.Sqrt((x1 - x0) ** 2.0 + (y1 - y0) ** 2.0)

        if d > r0 + r1 then
            []
        elif d < abs (r0 - r1) then
            []
        elif d = 0.0 && r0 = r1 then
            []
        else
            let a =
                (r0 ** 2.0 - r1 ** 2.0 + d ** 2.0) / (2.0 * d)

            let h = Math.Sqrt(r0 ** 2.0 - a ** 2.0)
            let x2 = x0 + a * (x1 - x0) / d
            let y2 = y0 + a * (y1 - y0) / d
            let x3 = x2 + h * (y1 - y0) / d
            let y3 = y2 - h * (x1 - x0) / d
            let x4 = x2 - h * (y1 - y0) / d
            let y4 = y2 + h * (x1 - x0) / d
            [{X = x3; Y = y3}; {X = x4; Y = y4}]

    let inline distance a b = sqrt ((a.X - b.X)**2. + (a.Y - b.Y)**2.0)
    // let inline distance a1 a2 b1 b2 = sqrt ((a1 - b1)**2. + (a2 - b2)**2.0)

    let absolutePosition (tag: TagData): Position =
        let pos = 
            List.allPairs tag.DetectorDistances tag.DetectorDistances
            |> List.where ^ fun (a, b) -> a <> b
            |> List.collect ^ fun (a, b) -> intersectionPoints (a.Origin, a.Distance) (b.Origin, b.Distance)
            |> List.groupBy ^ fun pos -> List.map (ceil >> int) <| [pos.X; pos.Y] 
            |> List.maxBy ^ fun (_, repeats) -> List.length repeats
        match pos with 
        | _, value :: _ -> value
        | _, _ -> failwith "No value found"

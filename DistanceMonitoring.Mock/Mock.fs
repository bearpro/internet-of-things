namespace DistanceMonitoring.Mock

open System
open DistanceMonitoring.Data.CommonTypes
open DistanceMonitoring.Utils

type Configuration =
  { Speed: float
    Labels: string list 
    Origins: Position list
    Bounds: Position
    Latency: int option }

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

    let absolutePosition distances: Position =
        List.allPairs distances distances
        |> List.where ^ fun (a, b) -> a <> b
        |> List.collect ^ fun (a, b) -> intersectionPoints (a.Origin, a.Distance) (b.Origin, b.Distance)
        |> List.groupBy id
        |> List.maxBy ^ fun (value, repeats) -> List.length repeats
        |> fun (value, _) -> value


module Instance =
    let private random = Random()
    
    /// <summary>
    /// Возвращает случайную координату в указанном диапазоне.
    /// </summary>
    /// <param name="bounds">Диапозон на который может упасть результат.</param>
    let randomPosition bounds = 
        { X = random.NextDouble() * bounds.X 
          Y = random.NextDouble() * bounds.Y }


    /// <summary>
    /// Возвращает случайный угол в радианах.
    /// </summary>
    let radomDirection () =
        random.NextDouble() * Math.PI * 2.

    let rotate {X = x; Y = y} angle =
        let x' = (cos angle) * x - (sin angle) * y
        let y' = (sin angle) * x + (sin angle) * y
        { X = x'; Y = y' }

    let inline clamp bound = function
        | x when x < 0.0 -> 0.0
        | x when x > bound -> bound
        | x -> x
        

    let updatePosition speed bounds (tag, pos) =
        let delta = rotate { X = speed; Y = 0. } (radomDirection())
        tag, {pos with X = pos.X + delta.X |> clamp bounds.X
                       Y = pos.Y + delta.Y |> clamp bounds.X }

    /// <summary>
    /// Конвертирует положение в список расстояний до опорных точек.
    /// </summary>
    /// <param name="tagPosition">Абсолютное положение.</param>
    /// <param name="origins">Положения опорных точек.</param>
    let distanceToOrigins tagPosition origins = 
        [ for originPosition in origins ->
            let x' = originPosition.X - tagPosition.X |> abs
            let y' = originPosition.Y - tagPosition.Y |> abs
            let r = sqrt (x' ** 2.0 + y' ** 2.0)
            { Origin = originPosition; Distance = r } ]
    
    /// <summary>
    /// Возвращает поток событий от контроллера.
    /// </summary>
    /// <param name="labels">Список наименований меток.</param>
    /// <param name="origins">Список координат датчиков.</param>
    /// <param name="bounds">Границы помещения.</param>
    /// <param name="speed">Скорость передвижения меток.</param>
    let setupStream config : seq<Async<TagData>> = 
        let mutable tags = 
            config.Labels 
            |> List.map ^ fun label -> label, randomPosition config.Bounds
        let next() = tags <- tags |> List.map ^ updatePosition config.Speed config.Bounds
        seq { 
            while true do 
                next ()
                for label, pos in tags do
                    let data = 
                      { Label = label
                        DetectorDistances = distanceToOrigins pos config.Origins }
                    yield async { 
                        printfn "%A -> %A\n%A" 
                            pos 
                            (distanceToOrigins pos config.Origins |> PositionCalculator.absolutePosition) 
                            (distanceToOrigins pos config.Origins) 

                        do! Async.Sleep ^ Option.defaultValue 50 config.Latency
                        return  data }
        }

namespace DistanceMonitoring.Mock

open System
open DistanceMonitoring.Data.CommonTypes
open DistanceMonitoring.Utils

type Configuration =
  { Speed: float
    Labels: string list 
    Origins: Position list
    Bounds: Position
    Latency: int option 
    DirectionChangeProbability: float option}

module Instance =
    let private random = Random()
    
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
        

    /// <summary>
    /// Обновляет положение, перемещая точку в случайную сторону.
    /// </summary>
    /// <param name="speed">Расстояние на которое переместится точка.</param>
    /// <param name="bounds">Верхняя граница, которую точка не пересекает.</param>
    /// <param name="tag">Метка.</param>
    /// <param name="pos">Исходное положение точки.</param>
    let updatePosition speed bounds direction (tag, pos) =
        let delta = rotate { X = speed; Y = 0. } direction
        tag, {pos with X = pos.X + delta.X |> clamp bounds.X
                       Y = pos.Y + delta.Y |> clamp bounds.X }
    
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
        let mutable directions = tags |> List.map ^ fun (t, _) -> t, radomDirection() 
        let next() = 
            directions <- directions
                |> List.map ^ fun (tag, dir) ->
                    let p = Option.defaultValue 0.01 config.DirectionChangeProbability
                    if random.NextDouble() <= p 
                    then tag, radomDirection()
                    else tag, dir
            let direction t = directions |> List.find ^ fun (t', _) -> t' = t 
                                         |> fun (_, d) -> d
            tags <- tags 
                    |> List.map ^ fun (tag, pos) -> 
                        updatePosition config.Speed config.Bounds (direction tag) (tag, pos)
        
        seq { 
            while true do 
                next ()
                for label, pos in tags do
                    let data = 
                      { Label = label
                        DetectorDistances = distanceToOrigins pos config.Origins }
                    yield async { 
                        do! Async.Sleep(Option.defaultValue 50 config.Latency)
                        return  data }
        }

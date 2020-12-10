namespace DistanceMonitoring.Web.Services
open System.Text
open DistanceMonitoring.Data.CommonTypes
open DistanceMonitoring.Web.Models
open Microsoft.Extensions.Logging

type DeviceData(log: ILogger<DeviceData>, mqtt: MqttServer) =
    let mutable tags = Map.empty
    let mutable lastOrigins = []
    let messageHandler (tag) = 
        log.LogInformation(sprintf "Tag received:\n%A" tag)
        let position = PositionCalculator.absolutePosition tag
        log.LogInformation(sprintf "Absolute position is :\n%A" position)
        let label = tag.Label
        if tags.ContainsKey label then
            tags <- tags.Remove(label)
                        .Add(label, position)
        else
            tags <- tags.Add(label, position)
        let origins = tag.DetectorDistances |> List.map (fun x -> x.Origin)
        if lastOrigins <> origins then
            log.LogInformation "Origins setup:"
            for o in origins do
                log.LogInformation(sprintf "X = %.2f, Y =%.2f)" o.X o.Y)
            lastOrigins <- origins

    do
        mqtt.SubscribeOnMessages messageHandler
        log.LogInformation("Subscribed on MQTT messages")
    with
        member _.Tags with get() =
            let tags' = 
                tags
                |> Map.toList
                |> List.map (fun (label, pos) -> { Label = label; Position = pos})
            tags'

        member _.Origins with get() = lastOrigins
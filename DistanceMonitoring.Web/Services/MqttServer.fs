namespace DistanceMonitoring.Web.Services
open System.Text
open MQTTnet
open MQTTnet.Server
open DistanceMonitoring.Data
open Microsoft.Extensions.Logging

type MqttServer(log: ILogger<MqttServer>) =
    let server = MqttFactory().CreateMqttServer()

    let mutable lastMessage: TagData option = None

    let mutable subscribers: (TagData -> unit) list = []

    let receiveMessage (message: MqttApplicationMessageInterceptorContext) =
        log.LogInformation(sprintf "MQTT message received from '%s' to topic '%s'" 
            message.ClientId 
            message.ApplicationMessage.Topic)
        
        let json = Encoding.UTF8.GetString message.ApplicationMessage.Payload
        let data = Serializer.deserializeFrom Json json
        lastMessage <- Some data
        for subscriber in subscribers do
            subscriber data
            

    let options = 
        MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .WithDefaultEndpointPort(1883)
            .WithApplicationMessageInterceptor(receiveMessage)
            .Build()

    let runAsync () = async {
        let task = server.StartAsync options
        do! Async.AwaitTask task
    }

    do 
        runAsync() |> Async.Start
        log.LogInformation("MQTT consumer started")

    member this.LastMessage with get() = lastMessage
    member this.SubscribeOnMessages(dispatcher: TagData -> unit) = 
        subscribers <- dispatcher :: subscribers
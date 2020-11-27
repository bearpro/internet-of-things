module DistanceMonitoring.CLI.MqttClient

open System.Text
open MQTTnet
open MQTTnet.Client
open MQTTnet.Client.Options
open DistanceMonitoring
open DistanceMonitoring.Data
open DistanceMonitoring.CLI.CommandLineArgs

let client = MqttFactory().CreateMqttClient()

let clientOptions options = 
    MqttClientOptionsBuilder()
        .WithClientId(options.ClientId)
        .WithTcpServer(options.Ip)
        .Build()

let main options =
    client.ConnectAsync(clientOptions options) 
    |> Async.AwaitTask
    |> Async.RunSynchronously
    |> ignore
    let labels = ["label-01"; "label-02"]
    let origins = [{X = 0.; Y = 0.}; {X = 3.; Y = 3.}; {X = 3.; Y = 0.}]
    for asyncItem in Mock.Instance.setupStream labels origins do
        async { 
            let! item = asyncItem 
            let json = Serializer.serializeTo Json item
            let payload = Encoding.UTF8.GetBytes json
            let! _ = client.PublishAsync("tokens", payload) |> Async.AwaitTask
            printfn "[*] Payload sent"
            ()
        }
        |> Async.RunSynchronously

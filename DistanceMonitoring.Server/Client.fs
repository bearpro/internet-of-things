module Client

open MQTTnet
open MQTTnet.Client
open MQTTnet.Client.Options
open System.Text
open System.Threading.Tasks

let client = MqttFactory().CreateMqttClient()

let options = 
    MqttClientOptionsBuilder()
        .WithClientId("bearpro")
        .WithTcpServer("192.168.1.21")
        .Build()

let runAsync () = async {
    let! _ = 
        client
            .UseApplicationMessageReceivedHandler(fun messaage -> 
                printfn "%s\n%s" 
                    messaage.ApplicationMessage.Topic 
                    (Encoding.UTF8.GetString messaage.ApplicationMessage.Payload))
            .ConnectAsync options 
            |> Async.AwaitTask
    let! _ = client.PublishAsync("distance_monitoring", Encoding.UTF8.GetBytes "Hello world") |> Async.AwaitTask
    return ()
}

let stopAsync = client.DisconnectAsync() |> Async.AwaitTask
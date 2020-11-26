module Server

open MQTTnet
open MQTTnet.Server
open System.Text

let server = MqttFactory().CreateMqttServer()

let options = 
    MqttServerOptionsBuilder()
        .WithDefaultEndpoint()
        .WithConnectionValidator(fun x -> printfn "%s:%s" x.Username x.Password )
        .WithApplicationMessageInterceptor(fun messaage -> 
            printfn "%s\n%s" 
                messaage.ApplicationMessage.Topic 
                (Encoding.UTF8.GetString messaage.ApplicationMessage.Payload))
        .Build()

let runAsync () = async {
    let task = server.StartAsync options
    do! Async.AwaitTask task
}

let stopAsync = server.StopAsync >> Async.AwaitTask
module DistanceMonitoring.CLI.MqttClient

open System.Text
open MQTTnet
open MQTTnet.Client
open MQTTnet.Client.Options
open FSharp.Json
open DistanceMonitoring
open DistanceMonitoring.Data
open DistanceMonitoring.Data.CommonTypes
open DistanceMonitoring.CLI.CommandLineArgs
open System.IO
open System
open HttpFs.Client
open Hopac

let client = MqttFactory().CreateMqttClient()

let clientOptions options = 
    MqttClientOptionsBuilder()
        .WithClientId(options.ClientId)
        .WithTcpServer(options.Ip)
        .Build()

/// <summary>
/// Проверяет доступность сервера.
/// </summary>
/// <returns></returns>
let probeHost ip port =
    let url = sprintf "http://%s:%i/tags/" ip port
    printfn "%s" url
    let request = Request.createUrl Get url
    job {
        try
            let! resp = getResponse request
            return resp.statusCode = 200
        with e -> 
            printfn "%O" e
            return false
    } |> Job.toAsync
      |> Async.RunSynchronously
     
    
    

let main options =
    if (probeHost options.Ip 5000) then
        client.ConnectAsync(clientOptions options) 
        |> Async.AwaitTask
        |> Async.RunSynchronously
        |> ignore
        let config:Mock.Configuration = File.ReadAllText >> Json.deserialize <| options.ConfigPath
        
        for asyncItem in (Mock.Instance.setupStream config) do 
            Async.RunSynchronously <| async { 
                let! item = asyncItem 
                let json = Serialization.serializeTo Serialization.SerializationFormat.Json item
                let payload = Encoding.UTF8.GetBytes json
                let! _ = client.PublishAsync("tokens", payload) |> Async.AwaitTask
                ()
            }
    else failwith "Сервер не ответил."

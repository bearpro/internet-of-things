open System
open System.Text
open System.Threading

[<EntryPoint>]
let main argv =
    match argv with
    | [| "server" |] -> 
        Server.runAsync () |> Async.Start
        Console.ReadKey () |> ignore
        Server.stopAsync () |> Async.RunSynchronously
    | [| "client"; name |] -> 
        Client.runAsync () |> Async.RunSynchronously 
    | invalidCommand -> ()
    0
 
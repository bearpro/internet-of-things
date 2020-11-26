namespace DistanceMonitoring.CLI

open System
open CommandLine
open DistanceMonitoring
open DistanceMonitoring.Data
open DistanceMonitoring.CLI.CommandLineArgs

#nowarn "0025"

module Main =
        
    [<EntryPoint>]
    let main argv =
        let result = CommandLine.Parser.Default.ParseArguments<MockOptions, ShitOptions>(argv)
        match result with
        | :? Parsed<obj> as parsed -> 
            match parsed.Value with
            | :? MockOptions as mockOptions -> run MqttClient.main mockOptions
            | :? ShitOptions -> 1
        | :? NotParsed<obj> as notParsed -> 1

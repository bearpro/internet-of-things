module DistanceMonitoring.Web.App

open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open DistanceMonitoring.Web
open DistanceMonitoring.Web.Models

let tagsHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        let service = ctx.GetService<Services.DeviceData>()
        let tags = service.Tags
        let origins = service.Origins
        let overlappings = 
            List.allPairs tags tags
            |> List.where (fun (a, b) -> a <> b && 1.0 < Services.PositionCalculator.distance a.Position b.Position)
            |> List.collect (fun (a, b) -> [a.Label; b.Label])
            |> List.distinct
            
        let data = { Tags = tags; Origins = origins; OverlappingLabels = overlappings}
        ctx.GetService<ILogger<Tags>>().LogInformation(sprintf "Sending %A" data)
        json data next ctx
        
let indexHandler =
    let path = Path.Combine(Directory.GetCurrentDirectory(), "WebRoot", "index.html")
    htmlFile path
        

let webApp =
    choose [
        GET >=>
            choose [
                route "/" >=> indexHandler
                route "/tags/" >=> tagsHandler
            ]
        setStatusCode 404 >=> text "Not Found" ]

let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

let configureCors (builder : CorsPolicyBuilder) =
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader()
           |> ignore

let configureApp (app : IApplicationBuilder) =
    let env = app.ApplicationServices.GetService<IWebHostEnvironment>()
    (match env.EnvironmentName with
    | "Development" -> app.UseDeveloperExceptionPage()
    | _ -> app.UseGiraffeErrorHandler(errorHandler))
        //.UseHttpsRedirection()
        .UseCors(configureCors)
        .UseStaticFiles()
        .UseGiraffe(webApp)

let configureServices (services : IServiceCollection) =
    services.AddCors()    |> ignore
    services.AddGiraffe() |> ignore
    services.AddSingleton<Services.MqttServer>() |> ignore
    services.AddSingleton<Services.DeviceData>() |> ignore

let configureLogging (builder : ILoggingBuilder) =
    builder.AddConsole()
           .AddDebug() |> ignore

[<EntryPoint>]
let main args =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot     = Path.Combine(contentRoot, "WebRoot")
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(
            fun webHostBuilder ->
                webHostBuilder
                    .UseContentRoot(contentRoot)
                    .UseWebRoot(webRoot)
                    .Configure(Action<IApplicationBuilder> configureApp)
                    .ConfigureLogging(configureLogging)
                    .ConfigureServices(configureServices)
                    |> ignore)
        .Build()
        .Run()
    0
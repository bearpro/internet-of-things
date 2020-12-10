module DistanceMonitoring.Data.Serialization

open System
open System.IO
open System.Xml.Serialization
open FSharp.Json
open DistanceMonitoring.Data.CommonTypes

type SerializationFormat = Json | Xml | Html

[<Serializable>]
[<XmlRoot("Tag-Data")>]
type public TagDataSerialzed (tag: TagData) =
    let mutable guid = tag.Label
    let mutable detectors = 
        tag.DetectorDistances
        |> List.map (fun x -> x.Origin)
        |> Array.ofList
    let mutable distances =
        tag.DetectorDistances
        |> List.map (fun x -> x.Distance)
        |> Array.ofList

    new () = TagDataSerialzed { Label = ""; DetectorDistances = [] } 

    [<XmlElement("Tag-Guid")>]
    member public _.Guid
        with get () = guid 
        and set value = guid <- value
    [<XmlElement("Detector-Positions")>]
    member public _.Detectors
        with get () = detectors
        and set value = detectors <- value
    [<XmlElement("TagToDetector-Distances")>]
    member public _.Distances
        with get () = distances
        and set value = distances <- value
    member public this.ToRecord() =
        { Label = this.Guid
          DetectorDistances = 
            Array.map2 (fun o d -> { Origin = o; Distance = d }) (this.Detectors) (this.Distances)
            |> List.ofArray }

let private xmlFormatter = XmlSerializer(typeof<TagDataSerialzed>)

let private serializeToXml data =
    let data = TagDataSerialzed data
    use stream = new StringWriter()
    do xmlFormatter.Serialize(stream, data)
    stream.ToString()

let private deserializeFromXml payload =
    let stream = new StringReader(payload)
    let data = xmlFormatter.Deserialize(stream) :?> TagDataSerialzed
    let data = data.ToRecord ()
    data

let private removeXmlTag (xmlPayload: string) =
    xmlPayload.Replace("""<?xml version="1.0" encoding="utf-16"?>""", "")

let serializeTo = function
    | Json -> Json.serialize 
    | Xml -> serializeToXml
    | Html -> serializeToXml >> removeXmlTag

let deserializeFrom = function
    | Json -> Json.deserialize 
    | Xml -> deserializeFromXml
    | Html -> deserializeFromXml

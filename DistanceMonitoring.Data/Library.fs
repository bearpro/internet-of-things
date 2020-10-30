namespace DistanceMonitoring.Data

open System
open System.IO
open System.Xml.Serialization
open FSharp.Json

[<CLIMutable; Serializable>]
/// Представляет абсолютные координаты объекта.
type Position = { X: float; Y: float }

[<CLIMutable; Serializable>]
/// Информация о метке и её положении относительно 
/// детекторов.
type TagData = 
  { Guid: Guid
    DetectorDistances: {| origin: Position; distance: float |} list }

/// Возможные форматы сериализации полезной нагрузки.
type SerializationFormat = Json | Xml 

module Serializer = 
    [<Serializable>]
    type public TagDataSerialzed (tag: TagData) =
        let mutable guid = tag.Guid
        let mutable detectors = 
            tag.DetectorDistances
            |> List.map (fun x -> x.origin)
            |> Array.ofList
        let mutable distances =
            tag.DetectorDistances
            |> List.map (fun x -> x.distance)
            |> Array.ofList

        new () = TagDataSerialzed { Guid = Guid(); DetectorDistances = [] } 

        member public _.Guid
            with get () = guid 
            and set value = guid <- value
        member public _.Detectors
            with get () = detectors
            and set value = detectors <- value
        member public _.Distances
            with get () = distances
            and set value = distances <- value
        member public this.ToRecord() =
            { Guid = this.Guid
              DetectorDistances = 
                Array.map2 (fun o d -> {| origin = o; distance = d|}) (this.Detectors) (this.Distances)
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

    let serializeTo = function
        | Json -> Json.serialize 
        | Xml -> serializeToXml

    let deserializeFrom = function
        | Json -> Json.deserialize 
        | Xml -> deserializeFromXml

[<AutoOpen>]
module Utils = 
    let (^) f x = f x
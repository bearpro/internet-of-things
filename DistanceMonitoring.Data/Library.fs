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
    let private serializeToXml<'a> (data: 'a) =
        let t = typeof<'a>
        let formatter = XmlSerializer(t)
        use stream = new StringWriter()
        do formatter.Serialize(stream, data)
        stream.ToString ()

    let serializeTo = function
        | Json -> Json.serialize 
        | Xml -> serializeToXml


    let deserializeTo<'a> format data =
        match format with
        | Json -> Json.deserialize<'a> data
        | Xml -> failwith "Not supported"

[<AutoOpen>]
module Utils = 
    let (^) f x = f x
namespace DistanceMonitoring.Data.CommonTypes

open System
open System.Xml.Serialization

[<CLIMutable; Serializable>]
/// Представляет абсолютные координаты объекта.
type Position = 
  { [<XmlElement("Position-X")>] X: float
    [<XmlElement("Position-Y")>] Y: float }

[<CLIMutable; Serializable>]
type Pivot = { Origin: Position; Distance: float }

[<CLIMutable; Serializable>]
/// Информация о метке и её положении относительно 
/// детекторов.
type TagData = 
  { Label: string
    DetectorDistances: Pivot list }

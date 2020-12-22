namespace DistanceMonitoring.Web.Models

open System
open DistanceMonitoring.Data.CommonTypes

type Message =
  { Text : string }

type Tag = { Label: string; Position: Position }
type Tags = { Tags: Tag list; OverlappingLabels: string list; Origins: Position list }
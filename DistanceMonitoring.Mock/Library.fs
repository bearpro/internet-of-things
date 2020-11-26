namespace DistanceMonitoring.Mock

open System
open DistanceMonitoring.Data

module Instance =
    let stream origins dataf = seq { 
        while true do 
            yield async { 
                do! Async.Sleep 1000
                return dataf () }
    }
    
    let private dataSample guid origins = 
      { Guid = guid
        DetectorDistances = 
            origins 
            |> List.map ^ fun o -> 
                {| origin = o; distance = 0.0 |} }
    
    let setupStream () = 
        let guid = Guid.NewGuid()
        stream [] ^ fun () -> dataSample guid [ { X = 1.0; Y = 1.0 }
                                                { X = 0.0; Y = 2.0 } ]

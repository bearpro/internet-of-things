namespace DistanceMonitoring

[<AutoOpen>]
module Utils =
    let (^) f x = f x
    let run f x =
        try
            f x |> ignore
            1
        with e -> raise e
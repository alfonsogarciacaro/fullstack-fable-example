module Helpers

open Fable.Core.JsInterop
open Fable.Import
open Thoth.Json

type SimpleHandler = express.Request -> express.Response -> unit

type express.Express with
    member app.Use(handler: express.RequestHandler) =
        app.``use``(handler) |> ignore
        app

type express.Request with
    member req.GetParam(key: string): string =
        req.``params``?(key)
    member req.GetParamAsInt(key: string): int =
        let p: string = req.``params``?(key)
        int p
    member inline req.ParseBody(decoder: Decode.Decoder<'T>): 'T =
        match Decode.fromValue "$" decoder req.body with
        | Ok v -> v
        | Error err -> failwith err

module Express =
    let errorHandler (res: express.Response) (ex: System.Exception) =
        printfn "ERROR: %s" ex.Message
        res.status(500).send(ex.Message) |> ignore

    let get (route: string) (handler: SimpleHandler) (app: express.Express) =
        app.get(!^route, (fun req res _ ->
            try
                handler req res
            with ex -> errorHandler res ex
        ))
        app

    let post (route: string) (handler: SimpleHandler) (app: express.Express) =
        app.post(!^route, (fun req res _ ->
            try
                handler req res
            with ex -> errorHandler res ex
        ))
        app

    let put (route: string) (handler: SimpleHandler) (app: express.Express) =
        app.put(!^route, (fun req res _ ->
            try
                handler req res
            with ex -> errorHandler res ex
        ))
        app

module Response =
    let sendString (body: string) (res: express.Response) =
        res.send(body) |> ignore

    let sendJson (body: 'a) (res: express.Response) =
        res.setHeader("Content-Type", !^"application/json")
        res.send(Encode.Auto.toString(0, body)) |> ignore

module Args =
    let findOrDefault k def =
        Node.Globals.``process``.argv
        |> Seq.skip 2
        |> Seq.pairwise
        |> Seq.tryPick (fun (k2, v) -> if k = k2 then Some v else None)
        |> Option.defaultValue def

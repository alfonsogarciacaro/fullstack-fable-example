module Helpers

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.PowerPack
open Thoth.Json
open Shared.Types

type Log =
    [<Emit("console.error('[ERROR]', $0)")>]
    static member Error(ex: System.Exception): unit = jsNative
    [<Emit("console.log('[INFO]', $0...)")>]
    static member Info([<System.ParamArray>] msg: obj[]): unit = jsNative

type SimpleHandler = express.Request -> express.Response -> JS.Promise<unit>

type express.Express with
    member app.Use([<ParamArray>] handlers: express.RequestHandler[]) =
        app.``use``(handlers) |> ignore; app
    member app.Catch(errorHandler: express.ErrorRequestHandler) =
        app.``use``(errorHandler) |> ignore

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
    let handle (handler: SimpleHandler) req (res: express.Response) =
        let errorHandler (ex: System.Exception) =
            Log.Error(ex)
            res.status(500).send(ex.Message) |> ignore
        try
            handler req res
            |> Promise.tryStart errorHandler
        with ex -> errorHandler ex

    let get (route: string) (handler: SimpleHandler) (app: express.Express) =
        app.get(!^route, (fun req res _ ->
            handle handler req res))
        app

    let post (route: string) (handler: SimpleHandler) (app: express.Express) =
        app.post(!^route, (fun req res _ ->
            handle handler req res))
        app

    let put (route: string) (handler: SimpleHandler) (app: express.Express) =
        app.put(!^route, (fun req res _ ->
            handle handler req res))
        app

module Promise =
    let bindList (f: 'T->JS.Promise<'U>) (xs: JS.Promise<'T list>) = promise {
        let mutable li = []
        let! xs = xs
        for x in xs do
            let! y = f x
            li <- y::li
        return List.rev li
    }

module Result =
    let unwrap = function
        | Ok v -> v
        | Error msg -> failwith msg

    let mapList f xs =
        (Ok [], xs) ||> Seq.fold (fun acc x ->
            match acc with
            | Error _ -> acc
            | Ok acc ->
                match f x with
                | Error er -> Error er
                | Ok x -> x::acc |> Ok)
        |> Result.map List.rev

module Response =
    let sendString (res: express.Response) (body: string) =
        res.send(body) |> ignore
        Promise.lift ()

    let sendJson (res: express.Response) (body: JS.Promise<'a>) =
        body |> Promise.map (fun body ->
            let json = Encode.Auto.toString(0, body, true)
            res.setHeader("Content-Type", !^"application/json")
            res.send(json) |> ignore)

module Args =
    let findOrDefault k def =
        Node.Globals.``process``.argv
        |> Seq.skip 2
        |> Seq.pairwise
        |> Seq.tryPick (fun (k2, v) -> if k = k2 then Some v else None)
        |> Option.defaultValue def

type Database private (client, database: CosmosDb.Database) =

    static member Create(endpoint, masterKey, dbId, init: Database->JS.Promise<unit>): JS.Promise<Database> =
        promise {
            let client = CosmosDb.Exports.CosmosClient.Create(jsOptions(fun o ->
                o.endpoint <- endpoint
                o.auth <- jsOptions(fun o -> o.masterKey <- Some masterKey)
            ))
            let! res = client.databases.createIfNotExists(jsOptions(fun o -> o.id <- dbId))
            let db = Database(client, res.database)
            do! init db
            return db
        }

    member __.CreateCollectionIfNotExists(collectionId: string) = promise {
        let! _res = database.containers.createIfNotExists(jsOptions(fun o -> o.id <- collectionId))
        return ()
    }

    member __.DeleteCollection(collectionId: string) =
        promise {
            let col = database.container(collectionId)
            let! _res = col.delete()
            return ()
        }

    member __.Read(collectionId: string, itemId: string, decoder: Decode.Decoder<'T>) =
        promise {
            let col = database.container(collectionId)
            let! res = col.item(itemId).read()
            match res.body with
            | None -> return "Item missing: " + itemId |> Error
            | Some x -> return Decode.fromValue "$" decoder x
        }

    member __.ReadAll(collectionId: string, decoder: Decode.Decoder<'T>): JS.Promise<Result<'T list, string>> =
        promise {
            let col = database.container(collectionId)
            let! res = col.items.readAll().toArray()
            match res.result with
            | None -> return "Cannot read" |> Error
            | Some xs -> return xs |> Result.mapList (Decode.fromValue "$" decoder)
        }

    member __.Upsert(collectionId: string, item: 'T, decoder: Decode.Decoder<'T>) =
        promise {
            let col = database.container(collectionId)
            let item = Encode.Auto.toString(0, item, true) |> JS.JSON.parse
            let! res = col.items.upsert(item)
            match res.body with
            | None -> return "Upsert unsuccessful" |> Error
            | Some x -> return Decode.fromValue "$" decoder x
        }

    member __.Delete(collectionId: string, itemId: string) =
        promise {
            let col = database.container(collectionId)
            let! _res = col.item(itemId).delete()
            return ()
        }

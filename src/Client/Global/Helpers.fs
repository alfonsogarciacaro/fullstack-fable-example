module Helpers

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.PowerPack
open Fable.PowerPack.Fetch
open Thoth.Json
open Shared.Types

[<Global("fetch")>]
let private fetchNative (req: RequestInfo, init: RequestInit): JS.Promise<Response> = jsNative

let private fetchPrivate url decoder props =
    let url = "/api" + url
    fetchNative(RequestInfo.Url url, requestProps props)
    |> Promise.bind (fun response ->
        if response.Ok
        then response.text()
        else failwith (string response.Status + " " + response.StatusText + " for URL " + response.Url))
    |> Promise.map (fun txt ->
        match Decode.fromString decoder txt with
        | Ok v -> v
        | Error er -> failwith er)

let fetch url (decoder: Decode.Decoder<'T>): JS.Promise<'T> =
    fetchPrivate url decoder []

let post<'T> url (body: 'T) decoder: JS.Promise<'T> =
    let props =
        [ RequestProperties.Method HttpMethod.POST
          requestHeaders [ContentType "application/json"]
          RequestProperties.Body !^(Encode.Auto.toString 0 body)]
    fetchPrivate url decoder props
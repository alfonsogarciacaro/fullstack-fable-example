module Question.Show.Rest

open System
open Fable.PowerPack
open Thoth.Json
open Shared.Types
open Shared.Types.Decoders
open Types
open Helpers

let getUser (id : string) =
    fetch (sprintf "/user/%s" id) User.Decoder

let getDetails (id : string) =
    fetch (sprintf "/question/%s" id) QuestionShow.Decoder

let createAnswer (questionId : string, userId : string, content : string) =
    promise {
        let! user = getUser userId
        let answer =
            { Id = Guid.NewGuid() |> string
              CreatedAt = DateTime.Now.ToString("O")
              Author = user
              Content = content
              Score = 0 }
        let url = sprintf "/question/%s/answer" questionId
        return! post url answer Answer.Decoder
    }

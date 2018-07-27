module Question.Show.Rest

open System
open Fable.PowerPack
open Thoth.Json
open Shared.Types
open Shared.Types.Decoders
open Types
open Helpers

let getUser (id : int) =
    fetch (sprintf "/user/%i" id) User.Decoder

let getDetails (id : int) =
    fetch (sprintf "/question/%i" id) QuestionShow.Decoder

let createAnswer (questionId : int, userId : int, content : string) =
    promise {
        let! user = getUser userId
        let answer =
            { Id = 0
              CreatedAt = DateTime.Now.ToString("O")
              Author = user
              Content = content
              Score = 0 }
        let url = sprintf "/question/%i/answer" questionId
        return! post url answer Answer.Decoder
    }

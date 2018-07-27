module Question.Index.Rest

open Thoth.Json
open Shared.Types
open Shared.Types.Decoders
open Helpers

let getQuestions _ =
    fetch "/question/list" (Decode.array Question.Decoder)

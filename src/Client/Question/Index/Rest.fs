module Question.Index.Rest

open Thoth.Json
open Helpers

let getQuestions _ =
    fetch "/question/list" (Decode.array questionDecoder)

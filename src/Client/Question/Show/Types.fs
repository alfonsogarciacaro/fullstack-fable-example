module Question.Show.Types

open Shared.Types

type Model =
    { QuestionId : int
      Question : Question option
      Answers : Answer.Types.Model list
      Reply : string
      Error : string
      IsWaitingReply : bool }

    static member Empty id =
        { QuestionId = id
          Question = None
          Answers = []
          Reply = ""
          Error = ""
          IsWaitingReply = false }

type GetDetailsRes =
    | Success of QuestionShow
    | Error of exn

type CreateAnswerRes =
    | Success of Answer
    | Error of exn

type Msg =
    | GetDetails of int
    | GetDetailsResult of GetDetailsRes
    | ChangeReply of string
    | Submit
    | CreateAnswerResult of CreateAnswerRes
    | AnswerMsg of int * Answer.Types.Msg

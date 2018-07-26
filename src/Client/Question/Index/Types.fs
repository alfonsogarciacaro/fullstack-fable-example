module Question.Index.Types

open Shared.Types

type Model =
    { Questions : Question[] option }

    static member Empty =
        { Questions = None }

type GetQuestionsRes =
    | Success of Question[]
    | Error of exn

type Msg =
    | GetQuestions
    | GetQuestionsResult of GetQuestionsRes

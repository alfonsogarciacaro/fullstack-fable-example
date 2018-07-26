module Question.Show.State

open Elmish
open Shared.Types
open Types

let init id =
    Model.Empty id , Cmd.ofMsg (GetDetails id)

let update (user : User) msg (model: Model) =
    match msg with
    | GetDetails id ->
        model, Cmd.ofPromise Rest.getDetails id (GetDetailsRes.Success >> GetDetailsResult) (GetDetailsRes.Error >> GetDetailsResult)

    | GetDetailsResult result ->
        match result with
        | GetDetailsRes.Success question ->
            // We use mutable here because it's easier to split the model and cmd, also performance isn't a problem here
            let mutable answersModel = []
            let mutable answersCmd = []

            for answer in question.Answers do
                let (subModel, subCmd) = Answer.State.init question.Question.Id answer answer.Author
                answersModel <- answersModel @ [subModel]
                answersCmd <- answersCmd @ [Cmd.map AnswerMsg subCmd]

            { model with Question = Some question.Question
                         Answers = answersModel}, Cmd.batch answersCmd

        | GetDetailsRes.Error error ->
            Logger.debugfn "[Question.Show.State] Error when fetching details: \n %A" error
            { model with Question = None }, Cmd.none

    | ChangeReply value ->
        { model with Reply = value }, Cmd.none

    | Submit ->
        if model.IsWaitingReply then
            model, Cmd.none
        else
            if model.Reply <> "" then
                { model with IsWaitingReply = true
                             Error = "" }, Cmd.ofPromise
                                                Rest.createAnswer
                                                (model.QuestionId, user.Id, model.Reply)
                                                (CreateAnswerRes.Success >> CreateAnswerResult)
                                                (CreateAnswerRes.Error >> CreateAnswerResult)
            else
                { model with Error = "Your answer can't be empty" }, Cmd.none

    | CreateAnswerResult result ->
        match result with
        | CreateAnswerRes.Error error ->
            Logger.debugfn "[Question.Show.State] Error when fetching details: \n %A" error
            { model with IsWaitingReply = false }, Cmd.none

        | CreateAnswerRes.Success answer ->
            let (answerModel, answerCmd) = Answer.State.init model.QuestionId answer answer.Author
            { model with Reply = ""
                         Error = ""
                         IsWaitingReply = false
                         Answers = model.Answers @ [answerModel] }, Cmd.map AnswerMsg answerCmd

    | AnswerMsg (rank,  msg) ->
        let mutable answersModel = []
        let mutable answersCmd = []

        model.Answers
        |> List.iteri(fun index answer ->
            if index = rank then
                let (subModel, subCmd) = Answer.State.update msg answer
                answersModel <- answersModel @ [subModel]
                answersCmd <- answersCmd @ [Cmd.map (fun x -> AnswerMsg (index, x)) subCmd]
            else
                answersModel <- answersModel @ [answer]
        )

        { model with Answers = answersModel }, Cmd.batch answersCmd

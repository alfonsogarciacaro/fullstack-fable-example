module Question.Dispatcher.View

open Types

let root user model dispatch =
    match model with
    | { CurrentPage = Router.QuestionPage.Index
        IndexModel = Some extractedModel } -> Question.Index.View.root extractedModel (IndexMsg >> dispatch)

    | { CurrentPage = Router.QuestionPage.Show _
        ShowModel = Some extractedModel } -> Question.Show.View.root user extractedModel (ShowMsg >> dispatch)

    | _ ->
        Render.pageNotFound

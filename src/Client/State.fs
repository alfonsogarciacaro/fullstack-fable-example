module App.State

open Elmish
open Types
open Fable.Import

let urlUpdate (result: Option<Router.Page>) model =
    match result with
    | None ->
        Browser.console.error("Error parsing url: " + Browser.window.location.href)
        model, Router.modifyUrl model.CurrentPage

    | Some page ->
        let model = { model with CurrentPage = page }
        match page with
        | Router.Question questionPage ->
            let (subModel, subCmd) = Question.Dispatcher.State.init questionPage
            { model with QuestionDispatcher = Some subModel }, Cmd.map QuestionDispatcherMsg subCmd
        | Router.Home ->
            let (subModel, subCmd) = Question.Dispatcher.State.init Router.QuestionPage.Index
            { model with QuestionDispatcher = Some subModel }, Cmd.map QuestionDispatcherMsg subCmd

let init user result =
    urlUpdate result (Model.Init user)

let update msg model =
    match (msg, model) with
    | (QuestionDispatcherMsg msg, { QuestionDispatcher = Some extractedModel }) ->
        let (subModel, subCmd) = Question.Dispatcher.State.update model.Session msg extractedModel
        { model with QuestionDispatcher = Some subModel }, Cmd.map QuestionDispatcherMsg subCmd

    | (QuestionDispatcherMsg capturedMsg, _) ->
        Browser.console.log("[App.State] Discarded message")
        printfn "%A" capturedMsg
        model, Cmd.none

    | (ToggleBurger, _) ->
        { model with IsBurgerOpen = not model.IsBurgerOpen }, Cmd.none

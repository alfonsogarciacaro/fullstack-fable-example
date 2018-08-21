module Router

open Fable.Import
open Fable.Helpers.React.Props
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser

let serverUrl path = "/api" + path

type QuestionPage =
    | Index
    | Show of int

type Page =
    | Question of QuestionPage
    | Home
    | Users

let toHash page =
    match page with
    | Question questionPage ->
        match questionPage with
        | Index -> "#question/index"
        | Show id -> sprintf "#question/%i" id
    | Home -> "#/"
    | Users -> "#users"

let pageParser: Parser<Page->Page,Page> =
    oneOf [
        map (Users) (s "users")
        map (QuestionPage.Index |> Question) (s "question" </> s "index")
        map (QuestionPage.Show >> Question) (s "question" </> i32)
        map (QuestionPage.Index |> Question) top ]

let href route =
    Href (toHash route)

let modifyUrl route =
    route |> toHash |> Navigation.modifyUrl

let newUrl route =
    route |> toHash |> Navigation.newUrl

let modifyLocation route =
    Browser.window.location.href <- toHash route

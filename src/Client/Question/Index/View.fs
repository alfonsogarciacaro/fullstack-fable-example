module Question.Index.View

open Shared.Types
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open Fulma.Extensions

let private loaderView isLoading =
    PageLoader.pageLoader [ PageLoader.IsActive isLoading ]
        [ ]

let private questionsView (question : Question) =
    let url =
        Router.QuestionPage.Show
        >> Router.Question

    Media.media [ ]
        [ Media.left [ ]
            [ Image.image [ Image.Is64x64 ]
                [ img [ Src ("avatars/" + question.Author.Avatar)  ] ] ]
          Media.content [ ]
            [ Heading.p [ Heading.IsSubtitle
                          Heading.Is5 ]
                [ a [ Router.href (url question.Id) ]
                    [ str question.Title ] ]
              Level.level [ ]
                [ Level.left [ ] [ ] // Needed to force the level right aligment
                  Level.right [ ]
                    [ Level.item [ ]
                        [ Help.help [ ]
                            [ str (sprintf "Asked by %s %s, %s"
                                                question.Author.Firstname
                                                question.Author.Surname
                                                question.CreatedAt) ] ] ] ] ] ]

let private questionsList questions =
    Columns.columns [ Columns.IsCentered ]
        [ Column.column [ Column.Width(Screen.All, Column.IsTwoThirds) ]
            (questions |> Array.map questionsView |> Array.toList) ]

let root (model: Types.Model) _ =
    match model.Questions with
    | Some questions ->
        Container.container [ ]
            [ loaderView false
              Section.section [ ]
                [ Columns.columns [ ]
                    [ Column.column [ Column.Width(Screen.All, Column.IsNarrow) ]
                        [ Heading.h3 [ ]
                            [ str "Latest questions" ] ]
                      Column.column [ ] [ ]
                    //   Column.column [ Column.Width (Screen.All, Column.IsNarrow) ]
                    //     [ Button.a [ Button.Color IsPrimary
                    //                  Button.Props [ Router.href (Router.Question Router.Create) ] ]
                    //         [ str "Ask a new question" ] ]
                    ] ]
              questionsList questions ]
    | None ->
        Container.container [ ]
            [ loaderView true ]

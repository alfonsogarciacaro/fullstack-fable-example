module Users

open System
open Fable.Import
open Shared.Types

module Types =
    type Model =
        { Users: User list option
          ButtonMessage: string }

    type Msg =
        | LoadUsers
        | FetchSuccess of User list
        | FetchFail of Exception

module State =
    open Types
    open Elmish
    open Thoth.Json
    open Shared.Types.Decoders

    let getUsers () =
        Helpers.fetch "/user/list" (Decode.list User.Decoder)

    let init () =
        { Users = None; ButtonMessage = "Load users" }

    let update msg model =
        match msg with
        | LoadUsers ->
            let cmd = Cmd.ofPromise getUsers () FetchSuccess FetchFail
            { model with ButtonMessage = "Loading users..." }, cmd
        | FetchFail error ->
            Browser.console.error("FATAL ERROR, RUN AWAY!!!!",error)
            model, Cmd.none
        | FetchSuccess users ->
            { model with Users = Some users }, Cmd.none

module View =
    open Fable.Helpers.React
    open Fable.Helpers.React.Props
    open Fulma
    open Fulma.Extensions
    open Types

    let private loaderView isLoading =
        PageLoader.pageLoader [ PageLoader.IsActive isLoading ]
            [ ]

    let private usersView (user : User) =
        Media.media [ ]
            [ Media.left [ ]
                [ Image.image [ Image.Is64x64 ]
                    [ img [ Src ("avatars/" + user.Avatar)  ] ] ]
              Media.content [ ]
                [ Level.level [ ]
                    [ Level.left [ ] [ ] // Needed to force the level right aligment
                      Level.right [ ]
                        [ Level.item [ ]
                            [ Help.help [ ]
                                [ str (sprintf "%s %s"
                                                    user.Firstname
                                                    user.Surname) ] ] ] ] ] ]

    let private usersList users =
        Columns.columns [ Columns.IsCentered ]
            [ Column.column [ Column.Width(Screen.All, Column.IsTwoThirds) ]
                (users |> List.map usersView) ]

    let root (model: Types.Model) dispatch =
        match model.Users with
        | Some users ->
            Container.container [ ]
                [ loaderView false
                  Section.section [ ]
                    [ Columns.columns [ ]
                        [ Column.column [ Column.Width(Screen.All, Column.IsNarrow) ]
                            [ Heading.h3 [ ]
                                [ str "Latest users" ] ]
                          Column.column [ ] [ ]
                        //   Column.column [ Column.Width (Screen.All, Column.IsNarrow) ]
                        //     [ Button.a [ Button.Color IsPrimary
                        //                  Button.Props [ Router.href (Router.Question Router.Create) ] ]
                        //         [ str "Ask a new user" ] ]
                        ] ]
                  usersList users ]
        | None ->
            Container.container [ ]
                [ Button.button
                    [Button.Disabled (model.ButtonMessage = "Loading users...")
                     Button.OnClick (fun _ -> dispatch LoadUsers) ]
                    [str model.ButtonMessage]
                 ]


module App.Types

open Shared.Types

type Author =
    { Id : int
      Firstname: string
      Surname: string
      Avatar : string }

type Question =
    { Id : int
      Author : Author
      Title : string
      Description : string
      CreatedAt : string }

type Model =
    { CurrentPage : Router.Page
      Session : SessionInfo
      QuestionDispatcher : Question.Dispatcher.Types.Model option
      IsBurgerOpen : bool }

    static member Init user =
        { CurrentPage =
            Router.QuestionPage.Index
            |> Router.Question
          Session = user
          QuestionDispatcher = None
          IsBurgerOpen = false }

type Msg =
    | QuestionDispatcherMsg of Question.Dispatcher.Types.Msg
    | ToggleBurger

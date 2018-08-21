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

type PageModel =
    | EmptyModel
    | QuestionModel of Question.Dispatcher.Types.Model
    | UserModel of Users.Types.Model

type Model =
    { CurrentPage : Router.Page
      Session : SessionInfo
      PageModel : PageModel
      IsBurgerOpen : bool }

    static member Init user =
        { CurrentPage =
            Router.QuestionPage.Index
            |> Router.Question
          Session = user
          PageModel = EmptyModel
          IsBurgerOpen = false }

type Msg =
    | UsersMsg of Users.Types.Msg
    | QuestionDispatcherMsg of Question.Dispatcher.Types.Msg
    | ToggleBurger

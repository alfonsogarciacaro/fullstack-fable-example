module Shared.Types

type User =
    { Id : int
      Firstname: string
      Surname: string
      Email: string
      Password: string
      Avatar : string
      Permissions : string }

type SignInData =
    { Email: string
      Password : string }

type SignInResponse =
    { Token : string
      User : User }

type SessionInfo = User // SignInResponse

type Question =
    { Id : int
      Author : User
      Title : string
      Description : string
      CreatedAt : string }

type Answer =
    { Id : int
      Score : int
      Author : User
      Content : string
      CreatedAt : string }

type QuestionShow =
    { Question : Question
      Answers : Answer list }

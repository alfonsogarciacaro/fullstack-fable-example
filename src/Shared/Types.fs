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

module Decoders =
  open Thoth.Json

  // Cache decoders
  let private userDecoder = Decode.Auto.generateDecoder<User>()
  let private signInDataDecoder = Decode.Auto.generateDecoder<SignInData>()
  let private answerDecoder = Decode.Auto.generateDecoder<Answer>()
  let private questionDecoder = Decode.Auto.generateDecoder<Question>()
  let private questionShowDecoder: Decode.Decoder<QuestionShow> =
      Decode.object (fun get ->
          { Question = get.Required.Field "Question" questionDecoder
            Answers = get.Required.Field "Answers" (Decode.list answerDecoder) }
      )

  type User with
    static member Decoder = userDecoder

  type SignInData with
    static member Decoder = signInDataDecoder

  type Answer with
    static member Decoder = answerDecoder

  type Question with
    static member Decoder = questionDecoder

  type QuestionShow with
    static member Decoder = questionShowDecoder

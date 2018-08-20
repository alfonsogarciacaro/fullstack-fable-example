module Shared.Types

open System

type User =
    { Id : string
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
    { Id : string
      Author : User
      Title : string
      Description : string
      CreatedAt : string }

type Answer =
    { Id : string
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
  let private userDecoder = Decode.Auto.generateDecoder<User>(true)
  let private signInDataDecoder = Decode.Auto.generateDecoder<SignInData>(true)
  let private answerDecoder = Decode.Auto.generateDecoder<Answer>(true)
  let private questionDecoder = Decode.Auto.generateDecoder<Question>(true)
  let private questionShowDecoder: Decode.Decoder<QuestionShow> =
      Decode.object (fun get ->
          { Question = get.Required.Field "question" questionDecoder
            Answers = get.Required.Field "answers" (Decode.list answerDecoder) }
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

// TODO: Fix until a new Thoth.Json version is released
module Encode =
    open Fable.Import
    open Fable.Core
    open Fable.Core.JsInterop
    open Thoth.Json

    [<Emit("Object.getPrototypeOf($0 || false) === Object.prototype")>]
    let private isObject x: bool = jsNative

    type Auto =
        static member toString(space : int, value : obj, ?forceCamelCase : bool) : string =
            let forceCamelCase = defaultArg forceCamelCase false
            JS.JSON.stringify(value, (fun _ value ->
                match value with
                // Match string before so it's not considered an IEnumerable
                | :? string -> value
                | :? System.Collections.IEnumerable ->
                    if JS.Array.isArray(value)
                    then value
                    else JS.Array.from(value :?> JS.Iterable<obj>) |> box
                // `isObject` includes arrays but these will be caught by previous branch
                | _ when forceCamelCase && isObject value ->
                    let replacement = createObj []
                    for key in JS.Object.keys value do
                        replacement?(key.[..0].ToLowerInvariant() + key.[1..]) <- value?(key)
                    replacement
                | _ -> value
            ), space)
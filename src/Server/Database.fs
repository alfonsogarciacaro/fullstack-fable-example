module Database

open System
open Fable.PowerPack
open Shared.Types
open Shared.Types.Decoders
open Helpers

type AnswerDb =
    { Id : string
      QuestionId : string
      AuthorId : string
      Content : string
      CreatedAt : string }

type QuestionDb =
    { Id : string
      AuthorId : string
      Title : string
      Description : string
      CreatedAt : string }

type DatabaseData =
    { Users : User []
      Questions : QuestionDb []
      Answers : AnswerDb [] }

module Decoders =
  open Thoth.Json

  let answerDb = Decode.Auto.generateDecoder<AnswerDb>(true)
  let questionDb = Decode.Auto.generateDecoder<QuestionDb>(true)

let private defaults() =
    let anonymous = Guid.Empty |> string
    let maxime = Guid.NewGuid() |> string
    let alfonso = Guid.NewGuid() |> string
    let robin = Guid.NewGuid() |> string
    let question1 = Guid.NewGuid() |> string
    let question2 = Guid.NewGuid() |> string

    {
      Users =
        [| { Id = anonymous
             Firstname = "Anonymous"
             Surname = ""
             Email = "anonymous@hotmail.com"
             Password = ""
             Avatar = "guest.png"
             Permissions = "" }
           { Id = maxime
             Firstname = "Maxime"
             Surname = "Mangel"
             Email = "mangel.maxime@fableconf.com"
             Password = "maxime"
             Avatar = "maxime_mangel.png"
             Permissions = "admin" }
           { Id = alfonso
             Firstname = "Alfonso"
             Surname = "Garciacaro"
             Email = "garciacaro.alfonso@fableconf.com"
             Password = "alfonso"
             Avatar = "alfonso_garciacaro.png"
             Permissions = "" }
           { Id = robin
             Firstname = "Robin"
             Surname = "Munn"
             Email = "robin.munn@fableconf.com"
             Password = "robin"
             Avatar = "robin_munn.png"
             Permissions = "" }
        |]
      Questions =
        [| { Id = question1
             AuthorId = robin
             Title = "What is the average wing speed of an unladen swallow?"
             Description =
                """
Hello, yesterday I saw a flight of swallows and was wondering what their **average wing speed** is.

If you know the answer please share it.
                """
             CreatedAt = "2017-09-14T17:44:28.103Z" }
           { Id = question2
             AuthorId = maxime
             Title = "Why did you create Fable ?"
             Description =
                """
Hello Alfonso,
I wanted to know why did you create Fable. Did you always planned to use F# ? Or was you thinking to others languages ?
                """
             CreatedAt = "2017-09-12T09:27:28.103Z" }
        |]
      Answers =
        [| { Id = Guid.NewGuid() |> string
             QuestionId = question1
             AuthorId = maxime
             Content =
                """
> What do you mean, an African or European Swallow ?
>
> Monty Python’s: The Holy Grail

Ok I must admit, I use google to search the question and found a post explaining the reference :).

I thought you was asking it seriously well done.
                """
             CreatedAt = "2017-09-14T19:57:33.103Z" }
           { Id = Guid.NewGuid() |> string
             QuestionId = question1
             AuthorId = alfonso
             Content =
                """
Maxime,

I believe you found [this blog post](http://www.saratoga.com/how-should-i-know/2013/07/what-is-the-average-air-speed-velocity-of-a-laden-swallow/).

And so Robin, the conclusion of the post is:

> In the end, it’s concluded that the airspeed velocity of a (European) unladen swallow is about 24 miles per hour or 11 meters per second.
                """
             CreatedAt = "2017-09-15T22:31:16.103Z" }
        |]
    }

type Database with
    member db.Users() = db.ReadAll("users", User.Decoder) |> Promise.map Result.unwrap
    member db.User(id: string) = db.Read("users", id, User.Decoder) |> Promise.map Result.unwrap
    member db.Questions() = db.ReadAll("questions", Decoders.questionDb) |> Promise.map Result.unwrap
    member db.Question(id: string) = db.Read("questions", id, Decoders.questionDb) |> Promise.map Result.unwrap
    member db.Answers() = db.ReadAll("answers", Decoders.answerDb) |> Promise.map Result.unwrap
    member db.Answer(id: string) = db.Read("answers", id, Decoders.answerDb) |> Promise.map Result.unwrap
    member db.UpsertUser(user: User) = db.Upsert("users", user, User.Decoder) |> Promise.map Result.unwrap
    member db.UpsertQuestion(question: QuestionDb) = db.Upsert("questions", question, Decoders.questionDb) |> Promise.map Result.unwrap
    member db.UpsertAnswer(answer: AnswerDb) = db.Upsert("answers", answer, Decoders.answerDb) |> Promise.map Result.unwrap


let init (db: Database) = promise {
    for colId in ["users"; "questions"; "answers"] do
        // do! db.DeleteCollection(colId)
        do! db.CreateCollectionIfNotExists(colId)
    let! users = db.Users()
    if List.isEmpty users then
        let defaults = defaults ()
        for user in defaults.Users do
            let! _ = db.UpsertUser user in ()
        for question in defaults.Questions do
            let! _ = db.UpsertQuestion question in ()
        for answer in defaults.Answers do
            let! _ = db.UpsertAnswer answer in ()
        Log.Info("Database initialized")
}
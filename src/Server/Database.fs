module Database

open Fable.Core.JsInterop
open Fable.Import
open Shared.Types

type AnswerDb =
    { Id : int
      QuestionId : int
      AuthorId : int
      Content : string
      CreatedAt : string }

type QuestionDb =
    { Id : int
      AuthorId : int
      Title : string
      Description : string
      CreatedAt : string }

type DatabaseData =
    { Users : User []
      Questions : QuestionDb []
      Answers : AnswerDb [] }

let dbFile = "./db.json"
let adapter = Lowdb.FileSyncAdapter(dbFile)

let mutable ``do not use directly db`` : Lowdb.Lowdb option = Option.None

type Database =
    static member Lowdb
        with get() : Lowdb.Lowdb =
            if ``do not use directly db``.IsNone then
                ``do not use directly db`` <- Lowdb.Lowdb(adapter) |> Some

            ``do not use directly db``.Value

    static member NextUserId
        with get() : int =
            let user: User =
                Database.Users
                    .sortBy("Id")
                    ?last()
                    ?value()
            user.Id + 1

    static member Users
        with get() : Lowdb.Lowdb =
            Database.Lowdb
                .get(!^"Users")

    static member Questions
        with get() : Lowdb.Lowdb =
            Database.Lowdb
                .get(!^"Questions")

    static member Answers
        with get() : Lowdb.Lowdb =
            Database.Lowdb
                .get(!^"Answers")

    static member NextQuestionId
        with get() : int =
            let question: QuestionDb =
                Database.Questions
                    .sortBy("Id")
                    ?last()
                    ?value()
            question.Id + 1

    static member NextAnswerId
        with get() : int =
            let answer: AnswerDb =
                Database.Answers
                    .sortBy("Id")
                    ?last()
                    ?value()
            answer.Id + 1

let writeDefaults () =
    Database.Lowdb
        .defaults(
            { Users =
                [| { Id = 0
                     Firstname = "Anonymous"
                     Surname = ""
                     Email = "anonymous@hotmail.com"
                     Password = ""
                     Avatar = "guest.png"
                     Permissions = "" }
                   { Id = 1
                     Firstname = "Maxime"
                     Surname = "Mangel"
                     Email = "mangel.maxime@fableconf.com"
                     Password = "maxime"
                     Avatar = "maxime_mangel.png"
                     Permissions = "admin" }
                   { Id = 2
                     Firstname = "Alfonso"
                     Surname = "Garciacaro"
                     Email = "garciacaro.alfonso@fableconf.com"
                     Password = "alfonso"
                     Avatar = "alfonso_garciacaro.png"
                     Permissions = "" }
                   { Id = 3
                     Firstname = "Robin"
                     Surname = "Munn"
                     Email = "robin.munn@fableconf.com"
                     Password = "robin"
                     Avatar = "robin_munn.png"
                     Permissions = "" }
                |]
              Questions =
                [| { Id = 1
                     AuthorId = 3
                     Title = "What is the average wing speed of an unladen swallow?"
                     Description =
                        """
Hello, yesterday I saw a flight of swallows and was wondering what their **average wing speed** is.

If you know the answer please share it.
                        """
                     CreatedAt = "2017-09-14T17:44:28.103Z" }
                   { Id = 2
                     AuthorId = 1
                     Title = "Why did you create Fable ?"
                     Description =
                        """
Hello Alfonso,
I wanted to know why did you create Fable. Did you always planned to use F# ? Or was you thinking to others languages ?
                        """
                     CreatedAt = "2017-09-12T09:27:28.103Z" }
                |]
              Answers =
                [| { Id = 1
                     QuestionId = 1
                     AuthorId = 1
                     Content =
                        """
> What do you mean, an African or European Swallow ?
>
> Monty Python’s: The Holy Grail

Ok I must admit, I use google to search the question and found a post explaining the reference :).

I thought you was asking it seriously well done.
                        """
                     CreatedAt = "2017-09-14T19:57:33.103Z" }
                   { Id = 2
                     QuestionId = 1
                     AuthorId = 2
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
        ).write()
    |> ignore

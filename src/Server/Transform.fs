module Transform

open Fable.Import
open Fable.PowerPack
open Database
open Helpers

let generateQuestion (db: Database) (questionDb : QuestionDb) : JS.Promise<Shared.Types.Question> = promise {
  let! author = db.User(string questionDb.AuthorId)
  return
    { Id = questionDb.Id
      Author =
        { Id = author.Id
          Firstname = author.Firstname
          Surname = author.Surname
          Email = author.Email
          Password = ""
          Avatar = author.Avatar
          Permissions = author.Permissions }
      Title = questionDb.Title
      Description = questionDb.Description
      CreatedAt = questionDb.CreatedAt }
}

let generateAnswer (db: Database) (answerDb : AnswerDb) : JS.Promise<Shared.Types.Answer> = promise {
  let! author = db.User(string answerDb.AuthorId)
  return
    { Id = answerDb.Id
      Author =
        { Id = author.Id
          Firstname = author.Firstname
          Surname = author.Surname
          Email = author.Email
          Password = ""
          Avatar = author.Avatar
          Permissions = author.Permissions }
      Content = answerDb.Content
      Score = 0
      CreatedAt = answerDb.CreatedAt }
}
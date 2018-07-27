module Server

open Fable.Core.JsInterop
open Fable.Import
open Shared.Types
open Shared.Types.Decoders
open Database
open Helpers

let publicPath = "./public"
let port = Args.findOrDefault "--port" "8080" |> int

let app = express.Invoke()

// Configure express application

app
// Register the static directories
// In development, static files will be served by webpack-dev-server
#if !DEBUG
    .Use(express.``static``.Invoke(publicPath, jsOptions(fun o ->
        o.index <- Some !^"index.html")))
#endif
    .Use(bodyParser.Globals.json())
    |> ignore

// Routing
// Prevent cache over JSON response
app.set("etag", false) |> ignore

app
|> Express.get "/api/status" (fun req res ->
    Response.sendString "Server is running!" res
)
|> Express.get "/api/user/list" (fun req res ->
    let users: User = Database.Users.value()
    Response.sendJson users res
)
|> Express.get "/api/user/:id" (fun req res ->
    let user: User =
        Database.Users
            .find(createObj [ "Id" ==> req.GetParamAsInt("id")])
            .value()
    Response.sendJson user res
)
|> Express.put "/api/user/:id/edit" (fun req res ->
    let user = req.ParseBody(User.Decoder)
    Database.Users
        .find(createObj [ "Id" ==> req.GetParamAsInt("id")])
        .assign(user)
        .write()
    Response.sendJson user res
)
|> Express.post "/api/user/create" (fun req res ->
    let user = { req.ParseBody(User.Decoder) with Id = Database.NextUserId }
    Database.Users
        .push(user)
        .write()
    Response.sendJson user res
)
|> Express.post "/api/sign-in" (fun req res ->
    let data = req.ParseBody(SignInData.Decoder)
    let user: User =
        Database.Users
            .find(createObj [
                    "Email" ==> data.Email
                    "Password" ==> data.Password
                ]).value()
    let data : SignInResponse =
        { Token = "I am a dummy token for now"
          User = { user with Password = "" }}
    Response.sendJson data res
)
|> Express.get "/api/question/list" (fun req res ->
    let questions: QuestionDb [] =
        Database.Questions
            .value()
    let questionsWithUser : Question [] =
        questions
        |> Array.map Transform.generateQuestion
    Response.sendJson questionsWithUser res
)
|> Express.get "/api/question/:id" (fun req res ->
    let questionDb: QuestionDb =
        Database.Questions
            .find(createObj [ "Id" ==> req.GetParamAsInt("id")])
            .value()
    let question : QuestionShow =
        { Question = Transform.generateQuestion questionDb
          Answers =
            Database.Answers
                .filter(createObj [ "QuestionId" ==> questionDb.Id ])
                .value<AnswerDb[]>()
            |> Array.map Transform.generateAnswer
            |> Array.toList
        }
    Response.sendJson question res
)
|> Express.post "/api/question/:id/answer" (fun req res ->
    let questionId = req.GetParamAsInt("id")
    let data: Answer = req.ParseBody(Answer.Decoder)
    let answer : AnswerDb =
        { Id = Database.NextAnswerId
          QuestionId = questionId
          AuthorId = data.Author.Id
          Content = data.Content
          CreatedAt = System.DateTime.UtcNow.ToString("O") }
    Database.Answers
        .push(answer)
        .write()
    let data: Answer = Transform.generateAnswer answer
    Response.sendJson data res
)
|> ignore

// Start the server
Database.writeDefaults()

app.listen(port, fun _ ->
    printfn "API Server started in port %i" port
) |> ignore

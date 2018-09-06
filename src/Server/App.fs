module Server

open System
open Fable.Import
open Fable.PowerPack
open Shared.Types
open Shared.Types.Decoders
open Database
open Helpers

let DB_HOST = ""
let DB_KEY = ""
let DB_ID = "fable-cosmosdb"
let PUBLIC_PATH = "./public"
let PORT = Args.findOrDefault "--port" "8080" |> int


let initApp (db: Database) =
    let app = express.Invoke()

    // Configure express application
    app.Use(
#if !DEBUG
        // Register the static directories
        // In development, static files will be served by webpack-dev-server
        express.``static``.Invoke(PUBLIC_PATH, jsOptions(fun o ->
            o.index <- Some !^"index.html")),
#endif
        // Parse body of JSON requests
        bodyParser.Globals.json()

        // Uncomment this to test errors from middleware
        // ,(fun req res next ->
        //     failwith "ERROR FROM MIDDLEWARE!")

    // Middleware error handling
    ).Catch(fun err req res next ->
        Log.Error(err)
        res.status(500).send(err.Message) |> ignore
    )

    // Routing
    // Prevent cache over JSON response
    app.set("etag", false) |> ignore

    app
    |> Express.get "/api/status" (fun req res ->
        Response.sendString res "Server is running!"
    )
    |> Express.get "/api/user/list" (fun req res ->
        db.Users()
        |> Response.sendJson res
    )
    |> Express.get "/api/user/:id" (fun req res ->
        req.GetParam("id")
        |> db.User
        |> Response.sendJson res
    )
    |> Express.put "/api/user/upsert" (fun req res ->
        req.ParseBody(User.Decoder)
        |> db.UpsertUser
        |> Response.sendJson res
    )
    |> Express.get "/api/question/list" (fun req res ->
        db.Questions()
        |> Promise.bindList (Transform.generateQuestion db)
        |> Promise.map List.toArray
        |> Response.sendJson res
    )
    |> Express.get "/api/question/:id" (fun req res ->
        promise {
            let! questionDb = req.GetParam("id") |> db.Question
            let! question = Transform.generateQuestion db questionDb
            let! answers =
                db.Answers()
                // TODO: Filter should be done in the database
                |> Promise.map (List.filter (fun x -> x.QuestionId = question.Id))
                |> Promise.bindList (Transform.generateAnswer db)
            return { Question = question; Answers = answers }
        }
        |> Response.sendJson res
    )
    |> Express.post "/api/question/:id/answer" (fun req res ->
        let questionId = req.GetParam("id")
        let data: Answer = req.ParseBody(Answer.Decoder)
        { Id = System.Guid.NewGuid() |> string
          QuestionId = questionId
          AuthorId = data.Author.Id
          Content = data.Content
          CreatedAt = System.DateTime.UtcNow.ToString("O") }
        |> db.UpsertAnswer
        |> Promise.bind (Transform.generateAnswer db)
        |> Response.sendJson res
    ) |> ignore

    app.listen(PORT, fun _ ->
        printfn "API Server started in port %i" PORT
    ) |> ignore

promise {
    let! db = Database.Create(DB_HOST, DB_KEY, DB_ID, Database.init)
    initApp db
} |> Promise.tryStart Log.Error

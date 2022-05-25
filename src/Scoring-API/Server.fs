module Server.API

open Saturn
open Giraffe.Core

open Environment.Reader
open Configuration

let appRouter : HttpHandler =
    router {
        not_found_handler (json "{ 'error': 'resource not found.' }")

        get "/health" (json "{ 'status': 'running' }")

        forward "/score" CpfScoring.Controller.apiRouter
    }

let app =
    application {
        use_router appRouter 
        url "http://0.0.0.0:8085"
        memory_cache
        use_static "static"
        use_gzip
        use_config (fun _ -> { connection = readConnectionStringFromEnvVars () })
    }

[<EntryPoint>]
let main _ =
    run app
    0

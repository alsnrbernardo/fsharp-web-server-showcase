namespace Migrations

open Environment.Reader

open Migrations.Runner

module Program =

    let logSuccess () = printfn "Database migration succeeded."

    let logError =
        function
        | UnsuccessfulUpgrade msg -> eprintfn "Could not migrate the database due to: %s" msg
        | DeploymentError msg -> eprintfn "Error while performing database migration: %s" msg

    [<EntryPoint>]
    let main _ =
        readConnectionStringFromEnvVars ()
        |> runScripts logSuccess logError

        0

namespace Migrations

open System.Reflection
open DbUp

module Runner =

    type MigrationFailure =
        | UnsuccessfulUpgrade of string
        | DeploymentError of string

    let migrateSchema assembly (connection: string) =
        try
            let upgrade =
                DeployChanges
                    .To
                    .PostgresqlDatabase(connection)
                    .WithVariablesDisabled()
                    .WithScriptsEmbeddedInAssembly(assembly)
                    .LogToConsole()
                    .Build()
                    .PerformUpgrade()

            if upgrade.Successful then
                Ok()
            else
                Error(UnsuccessfulUpgrade upgrade.Error.Message)
        with
        | ex -> Error(DeploymentError ex.Message)

    let migrateToLatest connection =
        let assembly = Assembly.GetExecutingAssembly()

        connection |> migrateSchema assembly

    let runScripts successFn errorFn connection =
        match migrateToLatest connection with
        | Ok value -> successFn value
        | Error failure -> errorFn failure

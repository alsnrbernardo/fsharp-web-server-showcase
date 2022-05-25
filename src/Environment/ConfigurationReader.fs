namespace Environment

open FsConfig

module Configuration =

    type LoggingLevel =
        | Disabled
        | Low
        | High

    type DatabaseProps =
        { User: string
          Password: string
          Host: string
          Port: string
          Type: string }

    type ServerProps = { Port: string }

    type Properties =
        { Database: DatabaseProps
          Server: ServerProps
          Logging: LoggingLevel }

module Reader =

    open Configuration

    let private readConfigFromEnvVars () =
        match EnvConfig.Get<Properties>() with
        | Ok config -> config
        | Error error ->
            match error with
            | NotFound envVarName -> failwithf "Variable %s not found." envVarName
            | BadValue (envVarName, value) -> failwithf "Variable %s has invalid value %s" envVarName value
            | NotSupported msg -> failwith msg

    let private config () = readConfigFromEnvVars ()

    let readConnectionStringFromEnvVars () =
        let db = config().Database

        let connStr =
            sprintf "Host=%s;Port=%s;Username=%s;Password=%s;Database=%s;" db.Host db.Port db.User db.Password db.Type

        match config().Logging with
        | Disabled -> connStr
        | Low
        | High -> connStr + "Include Error Detail=true;"

namespace Database.SQLMapper

open System.Linq

open Npgsql

open FSharp.Data.Sql.Common
open FSharp.Data.Sql

open Server.ApplicationError

module Query =

    let performAsyncQueryAllMap mapper query =
        async {
            try
                let! result = query |> Seq.executeQueryAsync

                return result |> mapper
            with
            | exn -> return exn |> DatabaseError |> Error
        }

module Command =

    let performInsertAsync createEntity =
        async {
            try
                let! result = createEntity ()
                return Ok result
            with
            | exn -> return exn |> DatabaseError |> Error
        }

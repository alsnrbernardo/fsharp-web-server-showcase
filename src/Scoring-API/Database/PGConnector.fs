namespace Database

open FSharp.Data.Sql
open Saturn
open Microsoft.AspNetCore.Http

module PGConnector =

    [<Literal>]
    let private CompileTimeConnection =
        "Host=postgres;Port=5432;Username=postgres;Password=admin;Database=postgres;"

    type private Schema =
        SqlDataProvider<ConnectionString=CompileTimeConnection, DatabaseVendor=Common.DatabaseProviderTypes.POSTGRESQL, UseOptionTypes=true>

    type DataSource = Schema.dataContext

    let createDBContext (connection: string) = Schema.GetDataContext connection

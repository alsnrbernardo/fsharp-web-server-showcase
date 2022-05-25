namespace CpfScoring

open Microsoft.AspNetCore.Http
open Saturn

open System.Threading.Tasks

open FsToolkit.ErrorHandling

open Server.ApplicationError

open Database.PGConnector

open Configuration

open CpfScoring.Repository
open CpfScoring.Domain

module Controller =

    [<RequireQualifiedAccess>]
    module Response =

        type Body = { value: int; created_at: string }

        type ErrorStatus =
            | BadRequest of string
            | ServerError of string

        let failToErrorResponse (failure: FailState) =
            match failure with
            | InvalidCpf -> BadRequest "The provided CPF is not valid."
            | ScoreOutOfValidRange -> ServerError "Unable to process request, please contact the system administrator"
            | DatabaseError _ -> ServerError "Internal error while processing the request."

        let handleErrorResponse ctx error =
            task {
                match error with
                | BadRequest msg -> return! Response.badRequest ctx msg
                | ServerError er -> return! Response.internalError ctx er
            }

    [<RequireQualifiedAccess>]
    module Request =

        type Body = { cpf: string }

        let toResponse (inst: ScoreInstance) : Response.Body =
            { value = Score.extract inst.value
              created_at = inst.created_at.ToString "dd/MM/yyyy HH:mm:ss" }

    let private createCpfScore (ds: DataSource) (input: Request.Body) =
        asyncResult {
            let! cpf = input.cpf |> CPF.create

            let! score = CpfScoring.Service.score cpf

            let! cpfScore = CpfScore.create score

            do! Command.insert ds cpfScore
        }

    let newCpfScore (ctx: HttpContext) : Task<HttpContext option> =
        task {
            let! input =
                Controller.getModel<Request.Body> ctx
                |> Async.AwaitTask

            let config = Controller.getConfig ctx

            let dataSource = createDBContext config.connection

            let! result = createCpfScore dataSource input

            match result with
            | Ok _ -> return! Response.ok ctx "New score for CPF registered!"
            | Error failure ->
                return!
                    Response.failToErrorResponse failure
                    |> Response.handleErrorResponse ctx
        }

    let private getAllScores (ds: DataSource) (input: string) =
        asyncResult {
            let! cpf = CPF.create input

            return! cpf |> Query.getAllByCpf ds
        }

    let listScoresByCpf (ctx: HttpContext) (input: string) : Task<HttpContext option> =
        task {
            let config = Controller.getConfig ctx

            let dataSource = createDBContext config.connection

            let! result = input |> getAllScores dataSource

            match result with
            | Ok scores ->
                return!
                    scores
                    |> Seq.map Request.toResponse
                    |> Response.ok ctx
            | Error failure ->
                return!
                    Response.failToErrorResponse failure
                    |> Response.handleErrorResponse ctx
        }

    let apiRouter =
        controller {
            show listScoresByCpf
            create newCpfScore
        }

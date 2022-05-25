namespace CpfScoring

open Server
open Server.ApplicationError

open CpfScoring.Domain

open Database.PGConnector

module Repository =

    type private CpfScoreEntity = DataSource.``public.cpfscoresEntity``

    module private CpfScoreEntity =

        let create (ds: DataSource) (score: CpfScore) : CpfScoreEntity =
            let entity = ds.Public.Cpfscores.Create()
            entity.Id <- score.id.ToString()
            entity.Cpf <- CPF.extract score.cpf
            entity.Value <- Score.extract score.value
            entity.CreatedAt <- score.created_at
            entity

        let toDomain (value, createdAt) : Result<ScoreInstance, FailState> =
            ScoreInstance.create (value) (createdAt)

    module Query =

        open Database.SQLMapper.Query

        let getAllByCpf (ds: DataSource) (cpf: CPF) : Async<Result<seq<ScoreInstance>, FailState>> =
            async {
                let criteria = CPF.extract cpf

                return!
                    query {
                        for score in ds.Public.Cpfscores do
                            where (score.Cpf = criteria)
                            select (score.Value, score.CreatedAt)
                    }
                    |> performAsyncQueryAllMap (fun entities ->
                        entities
                        |> Seq.map CpfScoreEntity.toDomain
                        |> Result.sequence)
            }

    module Command =

        open Database.SQLMapper.Command

        let insert (ds: DataSource) (score: CpfScore) : Async<Result<unit, FailState>> =
            async {
                return!
                    performInsertAsync (fun () ->
                        do (CpfScoreEntity.create ds score) |> ignore
                        ds.SubmitUpdatesAsync())
            }

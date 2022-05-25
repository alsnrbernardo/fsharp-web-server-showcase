namespace CpfScoring

open System

open Server.ApplicationError

module DTO =

    type CpfScore =
        { id: string
          cpf: string
          value: int
          created_at: System.DateTime }

module Domain =

    type CPF = private CPF of string

    type Score = private Score of int

    type CpfScore =
        { id: Guid
          cpf: CPF
          value: Score
          created_at: DateTime }

    type ScoredCPF = { cpf: CPF; value: Score }

    type ScoreInstance = { value: Score; created_at: DateTime }

    module CPF =

        let create (value: string) : Result<CPF, FailState> =
            if BrazilianUtils.Cpf.IsValid value then
                Ok(value |> String.filter Char.IsDigit |> CPF)
            else
                Error InvalidCpf

        let extract (CPF value) = value

    module Score =

        let create (value: int) =
            if value > 0 && value <= 1000 then
                Score value |> Ok
            else
                Error ScoreOutOfValidRange

        let extract (Score value) = value

    module ScoredCPF =

        let create (cpf: CPF) (value: int) : Result<ScoredCPF, FailState> =
            Score.create value
            |> Result.map (fun (score: Score) -> { cpf = cpf; value = score })

    module CpfScore =

        let create (scored: ScoredCPF) : Result<CpfScore, FailState> =
            Ok
                { id = Guid.NewGuid()
                  cpf = scored.cpf
                  value = scored.value
                  created_at = DateTime.Now }

        // TODO:: check for redundancy
        let extract (score: CpfScore) : DTO.CpfScore =
            { id = score.id.ToString()
              cpf = CPF.extract score.cpf
              value = Score.extract score.value
              created_at = score.created_at }

    module ScoreInstance =

        let create (value: int) (created_at: DateTime) : Result<ScoreInstance, FailState> =
            Score.create value
            |> Result.map (fun (score: Score) ->
                { value = score
                  created_at = created_at })

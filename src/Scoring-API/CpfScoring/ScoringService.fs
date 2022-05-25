namespace CpfScoring

open Server.ApplicationError

open CpfScoring.Domain

module Service =

    let private rng = System.Random()

    let score (cpf: CPF) : Async<Result<ScoredCPF, FailState>> =
        async { return ScoredCPF.create cpf (rng.Next(1, 1000)) }

namespace Server

module ApplicationError =

    type FailState =
        | InvalidCpf
        | ScoreOutOfValidRange
        | DatabaseError of exn

module Result =

    let prepend firstR restR =
        match firstR, restR with
        | Ok first, Ok restR -> Ok(Seq.append [ first ] restR)
        | Error e1, Ok _ -> Error e1
        | Ok _, Error e2 -> Error e2
        | Error e1, Error _ -> Error e1

    let sequence results =
        let zero = Ok Seq.empty
        Seq.foldBack prepend results zero

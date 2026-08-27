namespace Store

type private Store<'k, 'v when 'k: comparison>(?initialValues: Map<'k, 'v>) =
    let mutable all = defaultArg initialValues Map.empty

    member _.All = all |> Map.toSeq

    member _.Set id value =
        all <- Map.add id value all
        (id, value)

    member _.Read id =
        Map.tryFind id all |> Option.map (fun v -> (id, v))



type SequentialKeyStore<'v>(initialValues: Map<int, 'v>) =
    let store = Store(initialValues)

    member private this.nextId = (store.All |> Seq.map fst |> Seq.fold max 0) + 1

    member _.List = store.All

    member this.Add value = store.Set this.nextId value

    member _.Read k = store.Read k

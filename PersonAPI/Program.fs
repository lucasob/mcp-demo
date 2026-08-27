namespace PersonAPI

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging

open MCPShared
open Store

module Program =

    [<EntryPoint>]
    let main args =

        let builder = WebApplication.CreateBuilder(args)
        let app = builder.Build()

        // So we have something to start with
        let databaseSeed =
            Map
                [ (0,
                   { Name = "Lucas"
                     DateOfBirth = DateOnly(1996, 11, 29) }) ]

        let database = SequentialKeyStore(databaseSeed)

        app.MapGet("/ping", Func<string>(fun () -> "pong")) |> ignore

        app.MapGet(
            "/api/persons",
            Func<List<StoredItem<int, Person>>>(fun () ->
                app.Logger.LogInformation("Received list request")

                database.List
                |> Seq.map (fun (id, person) -> { Id = id; Value = person })
                |> Seq.toList)
        )
        |> ignore

        app.MapGet(
            "/api/persons/{id}",
            Func<int, IResult>(fun id ->
                app.Logger.LogInformation($"Requested person id={id}")

                match database.Read id with
                | Some(id, person) -> Results.Ok({ Id = id; Value = person })
                | None -> Results.NotFound())
        )
        |> ignore

        app.MapGet(
            "/api/persons/{id}/age",
            Func<int, IResult>(fun id ->
                app.Logger.LogInformation($"Requested age for person id={id}")

                match database.Read id with
                | Some(_, person) -> Results.Ok({ Age = person.Age() })
                | None -> Results.NotFound())
        )
        |> ignore

        app.MapPost(
            "/api/persons",
            Func<Person, IResult>(fun p ->
                app.Logger.LogInformation("Received new person")
                let id, person = database.Add p
                Results.Created("", { Id = id; Value = person }))
        )
        |> ignore

        app.Run()

        0

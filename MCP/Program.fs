open System
open System.ComponentModel
open System.Text
open System.Threading.Tasks
open System.Net.Http
open System.Net.Http.Json
open ModelContextProtocol.Server
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open MCPShared

type IPersonAPI =
    abstract ListPersons: unit -> Task<StoredItem<int, Person> list>
    abstract AddPerson: Person -> Task<StoredItem<int, Person>>
    abstract GetPerson: int -> Task<StoredItem<int, Person>>
    abstract GetAge: int -> Task<Age>

type PersonApi(client: HttpClient) =
    interface IPersonAPI with
        member _.ListPersons() =
            client.GetFromJsonAsync<StoredItem<int, Person> list>("persons")

        member _.GetAge(id) =
            client.GetFromJsonAsync<Age>($"persons/{id}/age")

        member _.GetPerson(id) =
            client.GetFromJsonAsync<StoredItem<int, Person>>($"persons/{id}")

        member _.AddPerson(p) =
            task {
                let asJson = System.Text.Json.JsonSerializer.Serialize(p)
                let content = new StringContent(asJson, Encoding.UTF8, "application/json")
                let! o = client.PostAsync("persons", content)

                match o.IsSuccessStatusCode with
                | true ->
                    let! returnedItem = o.Content.ReadFromJsonAsync<StoredItem<int, Person>>()
                    return returnedItem
                | false -> return failwith "dead"
            }



[<McpServerToolType>]
type PersonTools(api: IPersonAPI) =

    [<McpServerTool>]
    [<Description("List all persons")>]
    member _.ListPersons() : Task<StoredItem<int, Person> list> = api.ListPersons()

    [<McpServerTool>]
    [<Description("Get a person by id")>]
    member _.GetPerson(id: int) : Task<StoredItem<int, Person>> = api.GetPerson(id)

    [<McpServerTool>]
    [<Description("Get the age of a person by id")>]
    member _.GetAge(id: int) : Task<Age> = api.GetAge(id)

    [<McpServerTool>]
    [<Description("Create a new person")>]
    member _.SetPerson(p: Person) : Task<StoredItem<int, Person>> = api.AddPerson(p)

[<EntryPoint>]
let main argv =
    let builder = WebApplication.CreateBuilder(argv)

    builder.Services.AddHttpClient<IPersonAPI, PersonApi>(fun c -> c.BaseAddress <- Uri("http://localhost:5078/api/"))
    |> ignore

    builder.Services.AddMcpServer().WithHttpTransport().WithToolsFromAssembly()
    |> ignore

    let app = builder.Build()
    app.MapMcp() |> ignore

    app.Run("http://localhost:9000")

    0

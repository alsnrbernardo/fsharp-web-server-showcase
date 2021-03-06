# Scoring-API

POC API for scoring CPFs based on F# (Dotnet 6) and the Saturn Framework. 

# Frameworks/Libraries

- [Saturn](https://saturnframework.org/) & [Giraffe](https://giraffe.wiki/)
- [DbUp](https://dbup.readthedocs.io/en/latest/)
- [SqlProvider](https://fsprojects.github.io/SQLProvider/) (+ [Query Expressions](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/query-expressions))
- [FSharpPlus](https://fsprojects.github.io/FSharpPlus/) & [FsToolkit-ErrorHandling](https://demystifyfp.gitbook.io/fstoolkit-errorhandling)

### Setting the application up (from scratch)

- Pull and run the latest version of Postgres ([docker installation required](https://docs.docker.com/get-docker/))
```sh
docker run --name db-pgsql -e POSTGRES_PASSWORD=passw0rd -p 5432:5432 -d postgres
```
 - Clone the repository and move to its directory ([git installation required](https://www.atlassian.com/git/tutorials/install-git))
```sh
git clone https://github.com/alsnrbernardo/score-api.git && cd score-api
```
- Set up the dependencies
```sh
dotnet tool restore && dotnet paket install
```
- Build the solution
```
dotnet build .
```
- Perform database migration (depends on Postgres container)
```sh
dotnet run --project src/Migrations
```
- Run the application (available at http://localhost:8085)
```sh
dotnet run --project src/Scoring-API
```

### Testing

API endpoints:

* `GET /health` check if the server is running.
```
curl --request GET \
--url http://localhost:8085/health
```

* `GET /score/<CPF>` fetches the values and creation dates of scores attributed to the given [CPF](https://theonegenerator.com/generators/documents/cpf-generator/).
 ```
 curl --request GET \
--url http://localhost:8085/score/xxxxxxxxxxx \
--header 'Content-Type: application/json'
 ```

* `POST /score` scores and registers the provided CPF
```
curl --request POST \
--url http://localhost:8085/score \
--header 'Content-Type: application/json' \
--data '{ "cpf": "xxxxxxxxxxx" }}'
```

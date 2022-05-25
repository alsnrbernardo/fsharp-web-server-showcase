# score-api

POC API for CPF scoring based on F# and the SAFE stack

### Setting the application up (from scratch)

- Pull and run the latest version of Postgres ([docker installation required](https://docs.docker.com/get-docker/))
```sh
docker run --name db-pgsql -e POSTGRES_PASSWORD=passw0rd -p 5432:5432 -d postgres
```
 - Clone the repository and move to its directory ([git installation required](https://www.atlassian.com/git/tutorials/install-git))
```sh
git clone https://github.com/alsnrbernardo/score-api.git && cd score-api
```
- Build the score-api
```sh
dotnet tool restore
```
- Perform database migration (depends on Postgres container)
```sh
dotnet saturn migration
```
- Run the application (available at http://localhost:8085)
```sh
dotnet fake build -t run
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

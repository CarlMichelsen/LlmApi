# Llm Api
This is an api that allows for large language model promps to many different providers.
It supports streaming responses back in real time and the prompts have an identical format no matter what model you're prompting to.


## Publish consumer nuget package
dotnet pack -c Release YourProject.csproj


# Build docker image
```bash
docker build . -t llmapi:latest
```

# Database setup

## install entity framework cli tool
```bash
dotnet tool install --global dotnet-ef
```

## create migration if there are changes to the database
```bash
dotnet ef migrations add InitialCreate
```

## update database with latest migration
```bash
dotnet ef database update
```

## setup local docker database (for development)
make sure you have the *C:/database-data* folder on your pc

```bash
docker run -d --name development-database -e POSTGRES_PASSWORD=developer-password -v C:/database-data:/var/lib/postgresql/data --restart always -p 5432:5432 postgres
```
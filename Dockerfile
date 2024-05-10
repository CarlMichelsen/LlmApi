FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["llmapi.sln", "./"]

COPY . .

RUN dotnet restore

RUN dotnet test

RUN dotnet build "./Api" -c Release --output /app/build

FROM build AS publish

RUN dotnet publish "./Api" -c Release --output /app/publish

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final

WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "./Api.dll"]
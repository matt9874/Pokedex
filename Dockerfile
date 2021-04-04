FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app
EXPOSE 80

COPY Pokedex.API Pokedex.API
COPY Pokedex.Application Pokedex.Application
COPY Pokedex.Domain Pokedex.Domain
COPY Pokedex.Infrastructure Pokedex.Infrastructure
RUN dotnet restore Pokedex.API/Pokedex.API.csproj

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Pokedex.API.dll"]

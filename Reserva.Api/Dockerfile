# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln ./
COPY Reserva.Api/*.csproj ./Reserva.Api/
COPY Reserva.Entity/*.csproj ./Reserva.Entity/
COPY Reserva.Dto/*.csproj ./Reserva.Dto/
COPY Reserva.Domain/*.csproj ./Reserva.Domain/
COPY Reserva.Common/*.csproj ./Reserva.Common/
COPY Reserva.Application/*.csproj ./Reserva.Application/
COPY Reserva.Application.Abstractions/*.csproj ./Reserva.Application.Abstractions/
COPY Reserva.Repository/*.csproj ./Reserva.Repository/
COPY Reserva.Repository.Abstractions/*.csproj ./Reserva.Repository.Abstractions/

RUN dotnet restore

COPY . ./
WORKDIR /src/Reserva.Api
RUN dotnet publish -c Release -o /app/publish

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

RUN apt-get update && apt-get install -y --allow-unauthenticated \
    libc6-dev libgdiplus libx11-dev && rm -rf /var/lib/apt/lists/*

ENTRYPOINT ["dotnet", "Reserva.Api.dll"]
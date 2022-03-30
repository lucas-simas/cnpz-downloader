# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

#Copiando projetos necessários
COPY ../../cnpz-downloader ./
COPY ../../cnpz-kafka ./
WORKDIR /cnpz-downloader
#Compilando
RUN dotnet publish -c Release -o out

#Setando execução
FROM mcr.microsoft.com/dotnet/aspnet:6.0
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "aspnetapp.dll"]
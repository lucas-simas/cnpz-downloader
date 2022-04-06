# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

RUN mkdir ./cnpz-downloader
RUN mkdir ./cnpz-kafka
COPY ./cnpz-downloader ./cnpz-downloader
COPY ./cnpz-kafka ./cnpz-kafka
WORKDIR ./cnpz-downloader

#Compilando
RUN dotnet publish -c Release -o out

#Setando execução
FROM mcr.microsoft.com/dotnet/aspnet:6.0
COPY --from=build-env /cnpz-downloader/out .
ENTRYPOINT ["dotnet", "cnpz-downloader.dll"]
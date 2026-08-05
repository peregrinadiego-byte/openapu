FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore OpenAPU.sln
RUN dotnet publish src/OpenAPU.Api/OpenAPU.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV OPENAPU_DB_PATH=/data/openapu.db

VOLUME ["/data"]
EXPOSE 8080
USER app


ENTRYPOINT ["dotnet", "OpenAPU.Api.dll"]


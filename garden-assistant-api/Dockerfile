FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY garden-assistant-api/garden-assistant-api.csproj garden-assistant-api/
RUN dotnet restore garden-assistant-api/garden-assistant-api.csproj

COPY garden-assistant-api/ garden-assistant-api/
RUN dotnet publish garden-assistant-api/garden-assistant-api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
RUN groupadd -r appuser && useradd -r -g appuser appuser
USER appuser

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "garden-assistant-api.dll"]


# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source
COPY *.sln .
COPY awaitable-events/*.csproj ./awaitable-events/
RUN dotnet restore

# copy everything else and build app
COPY awaitable-events/. ./awaitable-events/
WORKDIR /source/awaitable-events
RUN dotnet publish -c release -o /app

RUN dotnet tool install --tool-path /tools dotnet-trace \
 && dotnet tool install --tool-path /tools dotnet-counters \
 && dotnet tool install --tool-path /tools dotnet-dump \
 && dotnet tool install --tool-path /tools dotnet-gcdump

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /tools
COPY --from=build /tools .
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "awaitable-events.dll"]
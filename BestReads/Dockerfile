# build and publish the app
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /App
COPY *.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release ./BestReads.csproj  -o out

# run the published app
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "BestReads.dll"]

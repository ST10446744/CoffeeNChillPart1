FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY CoffeeNChill_Functions.csproj .

RUN dotnet restore CoffeeNChill_Functions.csproj

COPY . .

RUN dotnet publish CoffeeNChill_Functions.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore


FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0

WORKDIR /home/site/wwwroot

COPY --from=build /app/publish .
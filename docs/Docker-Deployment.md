# CoffeeNChill Part 1 - Docker Deployment

## Docker Hub Images

Functions:
faizaan101/coffeenchill-functions:v1.0

Azurite:
faizaan101/coffeenchill-azurite:v1.0

## Run Azurite

docker run -d --name coffeenchill-azurite --network coffeenchill-network -p 10000:10000 -p 10001:10001 -p 10002:10002 mcr.microsoft.com/azure-storage/azurite

## Run Functions

docker run -d --name coffeenchill-functions --network coffeenchill-network -p 7071:80 coffeenchill-functions:v1.0

## API

The CoffeeNChill Functions expose menu management and staff document endpoints through HTTP.

## Testing

All eight required endpoints were tested successfully using Postman.

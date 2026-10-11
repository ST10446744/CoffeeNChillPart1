# CoffeeNChill Part 2 - Docker Deployment

## Docker Hub Image

The CoffeeNChill Azure Functions application is published to Docker Hub as:

`ghoste01/coffeenchill-functions:v2.0`

## Docker Compose

Part 2 uses Docker Compose to run the CoffeeNChill Functions application together with Azurite.

Azurite provides local emulation of Azure Blob Storage, Queue Storage and Table Storage.

The Docker Compose setup includes:

* CoffeeNChill Azure Functions
* Azurite Blob Storage
* Azurite Queue Storage
* Azurite Table Storage
* Azure Storage Queue for order processing
* Azure Table Storage for storing orders

## Start the Application

Open PowerShell in the CoffeeNChillPart1 project folder and run:

`docker compose up`

This starts both the Azurite storage services and the CoffeeNChill Functions application.

## Stop the Application

To stop the containers, run:

`docker compose down`

## Order Processing API

The Part 2 order endpoint is:

`POST http://localhost:7071/api/orders/queue`

Orders are placed into the `order-processing-queue` without blocking the customer request.

The queue-triggered `ProcessOrderQueue` function then processes the order and stores it in the `Orders` table.

## Order Status Lifecycle

Orders move through the following statuses:

`Received → Preparing → Ready → Collected`

## Testing

The order queue endpoint was tested using Postman.

A successful order submission returns HTTP status `202 Accepted`, confirming that the order has been accepted for asynchronous processing.

The Postman collection is included with the project as:

`CoffeeNChill_Part2_Postman_Collection.json`

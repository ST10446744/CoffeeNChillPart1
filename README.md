# CoffeeNChill — Azure Functions Ordering System

## 1. Project Overview

CoffeeNChill is a .NET 8 Azure Functions project that demonstrates how a coffee shop ordering system can use cloud technologies to manage menu items, staff documents, and customer orders.

The project was developed in two parts.

**Part 1** introduced the original menu management and staff document functionality.

**Part 2** extended the application with asynchronous order processing using Azure Storage Queues, Azure Table Storage, and Docker Compose.

The goal of Part 2 is to demonstrate how an application can accept orders quickly, place them in a queue, and process them separately in the background.

**GitHub Repository:** `ST10446744/CoffeeNChillPart1`

**Docker Hub Image:** `ghoste01/coffeenchill-functions:v2.0`

---

## 2. Part 1 — Existing Functionality

Part 1 established the original CoffeeNChill application.

The existing functionality includes:

* Retrieving all menu items.
* Retrieving menu items by category.
* Updating menu items.
* Uploading staff documents.
* Listing staff documents.
* Downloading staff documents.

These features are implemented in the Azure Functions project.

The main folders containing this functionality are:

* `CoffeeNChill.Functions/Functions/` — HTTP-triggered functions and application endpoints.
* `CoffeeNchill.Functions/Models/` — models representing application data.
* `CoffeeNchill.Functions/Services/` — services used by the application.

*Note: The actual project folder is named `CoffeeNChill.Functions`.*

---

## 3. Part 2 — Queue Triggers and Docker Compose Orchestration

Part 2 introduces a queue-based order processing workflow.

A coffee shop may receive many orders during busy university breaks. Processing every order completely during the initial HTTP request could make the application slower.

To address this, CoffeeNChill separates order submission from background processing.

### 3.1 How the Order Processing System Works

The workflow is:

1. A customer submits an order through an HTTP API.
2. The `QueueOrder` function receives the order.
3. The order is placed in an Azure Storage Queue.
4. The `ProcessOrderQueue` function receives the queued message.
5. The order is saved in Azure Table Storage.
6. The order status progresses through its lifecycle.

The queue used by the application is named `order-processing-queue`.

The Azure Table Storage table is named `Orders`.

### 3.2 New and Updated Files

The following files were added or updated for Part 2.

| File                                         | Location                            | Purpose                                                                                   |
| -------------------------------------------- | ----------------------------------- | ----------------------------------------------------------------------------------------- |
| `QueueOrder.cs`                              | `CoffeeNChill.Functions/Functions/` | Accepts HTTP order requests and adds them to the storage queue.                           |
| `ProcessOrderQueue.cs`                       | `CoffeeNChill.Functions/Functions/` | Processes queue messages and records orders in Azure Table Storage.                       |
| `OrderTableEntity.cs`                        | `CoffeeNChill.Functions/Models/`    | Defines the structure of an order record stored in Azure Table Storage.                   |
| `order.cs`                                   | `CoffeeNChill.Functions/Models/`    | Defines the order model used by the application.                                          |
| `docker-compose.yml`                         | Repository root                     | Starts the Azurite and Functions services together.                                       |
| `CoffeeNChill_Part2_Postman_Collection.json` | Repository root                     | Contains the exported Postman collection for API testing.                                 |
| `docs/Docker-Deployment.md`                  | `docs/`                             | Documents the Docker deployment process.                                                  |
| `.gitignore`                                 | Repository root                     | Excludes local configuration, runtime data, and other files that should not be committed. |
| `CoffeeNChill_Functions.csproj`              | `CoffeeNChill.Functions/`           | Contains the .NET project configuration and package references.                           |

### 3.3 Order Status Lifecycle

The order processing function updates an order through four statuses:

| Status    | Description                                 |
| --------- | ------------------------------------------- |
| Received  | The order has been accepted for processing. |
| Preparing | The order is being prepared.                |
| Ready     | The order is ready for collection.          |
| Collected | The order has been collected.               |

The current implementation includes short delays between status updates to demonstrate the lifecycle.

Because the updates happen quickly, the intermediate statuses may not remain visible for long.

### 3.4 Azure Table Storage

The `ProcessOrderQueue` function stores order records in the `Orders` table.

Each record includes information such as:

* Order ID.
* Customer name.
* Selected product SKUs.
* Total price.
* Order timestamp.
* Current order status.

The table uses the order date as the `PartitionKey` and the order ID as the `RowKey`.

These keys organise the records and help identify individual orders.

---

## 4. Docker Compose

Part 2 uses Docker Compose to run the application and its local storage services together.

The configuration is stored in:

`docker-compose.yml`

It defines two services.

| Service     | Purpose                                                                  |
| ----------- | ------------------------------------------------------------------------ |
| `azurite`   | Provides local Azure-compatible Blob, Queue, and Table Storage services. |
| `functions` | Runs the CoffeeNChill Azure Functions application.                       |

The Functions service uses the Docker Hub image:

`ghoste01/coffeenchill-functions:v2.0`

The application is accessible locally through port `7071`.

The Azurite services use the following ports:

* Blob Storage: `10000`
* Queue Storage: `10001`
* Table Storage: `10002`

### Starting the Application

Open PowerShell in the repository root:

```powershell
cd "C:\Users\faiza\Documents\CoffeeNChillPart1"
```

Start the application:

```powershell
docker compose up
```

To run the containers in the background:

```powershell
docker compose up -d
```

To check the running services:

```powershell
docker compose ps
```

To view the Functions logs:

```powershell
docker compose logs -f functions
```

To stop the services:

```powershell
docker compose down
```

Docker Desktop must be running before these commands are executed.

For more detailed instructions, see [`docs/Docker-Deployment.md`](docs/Docker-Deployment.md).

---

## 5. Submitting an Order

The Part 2 HTTP endpoint accepts order submissions.

**Endpoint:**

`POST http://localhost:7071/api/orders/queue`

In Postman, select:

* Method: `POST`
* URL: `http://localhost:7071/api/orders/queue`
* Body: `raw`
* Format: `JSON`

Example request:

```json
{
  "OrderId": "ORD-1001",
  "CustomerName": "Test Customer",
  "SelectedItemSKUs": [
    "COF-001",
    "MUF-002"
  ],
  "TotalPrice": 65.00,
  "OrderTimestamp": "2026-10-11T10:30:00"
}
```

Use a unique order ID for each test.

A successful submission should return:

`202 Accepted`

This indicates that the request was accepted for queueing. It does not, on its own, confirm that the order has been fully processed or saved in Table Storage.

The Functions logs and the `Orders` table should also be checked to verify the complete workflow.

---

## 6. Testing with Postman

The exported Postman collection is located in the repository root:

`CoffeeNChill_Part2_Postman_Collection.json`

To use it:

1. Start the application with Docker Compose.
2. Open Postman.
3. Import the JSON collection.
4. Run the order submission request.
5. Confirm that the API returns `202 Accepted`.
6. Check the Functions logs for queue processing messages.
7. Verify that the order is recorded in the `Orders` table.
8. Verify that the stored status progresses as expected.
9. Retest the existing Part 1 endpoints.

The status lifecycle and table records must be verified using actual test results. A successful HTTP response alone does not prove that every part of the workflow is working.

---

## 7. Project Structure

The main project structure is organised as follows:

```text
CoffeeNChillPart1/
│
├── README.md
├── docker-compose.yml
├── CoffeeNChill_Part2_Postman_Collection.json
│
├── docs/
│   └── Docker-Deployment.md
│
└── CoffeeNChill.Functions/
    │
    ├── Functions/
    │   ├── QueueOrder.cs
    │   ├── ProcessOrderQueue.cs
    │   ├── download staff documents.cs
    │   ├── get all menu items.cs
    │   ├── get menu items by category.cs
    │   ├── list staff documents.cs
    │   ├── update menu items.cs
    │   └── upload staff documents.cs
    │
    ├── Models/
    │   ├── MenuItem.cs
    │   ├── order.cs
    │   └── OrderTableEntity.cs
    │
    ├── Services/
    │   ├── document service.cs
    │   └── menu service.cs
    │
    ├── CoffeeNChill_Functions.csproj
    ├── Dockerfile
    ├── host.json
    └── program.cs
```

This tree highlights the main application and Part 2 files. Other project files may also exist.

Local runtime files, such as Azurite database files and trace logs, are excluded from the repository.

---

## 8. Technologies Used

The project uses the following technologies:

* **C#** — application programming language.
* **.NET 8** — development platform.
* **Azure Functions** — serverless-style function execution.
* **Azure Storage Queues** — asynchronous order message handling.
* **Azure Table Storage** — order record storage.
* **Azurite** — local emulator for Azure Storage services.
* **Docker** — containerisation of the application.
* **Docker Compose** — orchestration of the application and storage services.
* **Postman** — HTTP API testing.
* **Git and GitHub** — source control and repository hosting.
* **Docker Hub** — hosting the published Functions image.

---

## 9. Configuration and Security

The `local.settings.json` file is used for local Azure Functions configuration and is excluded from Git.

Do not commit real passwords, access tokens, private connection strings, or other secrets.

The `.gitignore` file also excludes local Azurite database files, the local Azurite configuration file, and the Core Tools trace log.

The current configuration is intended for local development and demonstration. A production deployment would require appropriate authentication, secure configuration, monitoring, and a production Azure Storage account.

---

## 10. Documentation and Demonstration

The project includes the following supporting files:

* **Deployment guide:** `docs/Docker-Deployment.md`
* **Part 2 Postman collection:** `CoffeeNChill_Part2_Postman_Collection.json`
* **Docker Compose configuration:** `docker-compose.yml`
* **Docker Hub image:** `ghoste01/coffeenchill-functions:v2.0`

**YouTube demonstration:** Add the demonstration link here after uploading the video.

The demonstration should show:

1. Starting the application with `docker compose up`.
2. The Functions host starting successfully.
3. Submitting an order through Postman.
4. The API returning `202 Accepted`.
5. The queue-triggered function processing the order.
6. The order appearing in Azure Table Storage.
7. The order status lifecycle.

Capture the required screenshots and include them in the assignment submission.

---

## 11. Changelog

### Part 2 — Queue Triggers & Docker Compose Orchestration

* Added an HTTP-triggered function for submitting orders to Azure Storage Queue.
* Added a queue-triggered function for processing orders asynchronously.
* Added an Azure Table Storage entity for recording orders.
* Updated the order model to include a status field.
* Implemented the order status lifecycle: Received, Preparing, Ready, and Collected.
* Added Docker Compose orchestration for Azurite and the Functions container.
* Published the Functions image as `ghoste01/coffeenchill-functions:v2.0`.
* Exported a Postman collection for testing.
* Updated the Docker deployment documentation.
* Updated `.gitignore` to exclude local runtime files and configuration.

### Part 1 — Original Application

* Implemented menu retrieval and category filtering.
* Added menu item updates.
* Implemented staff document upload, listing, and download functionality.
* Established the original Azure Functions project structure.


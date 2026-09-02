\# CoffeeNChill Canteen Management System – Part 1



\## 1. Project Overview



CoffeeNChill is a cloud-enabled Canteen Management System developed as part of Part 1 of the Portfolio of Evidence (PoE).



The system uses Azure Functions to provide HTTP-based microservices for managing menu items and staff documents. Azure Table Storage is used for menu data, while Azure File Storage is used for staff documents.



For local development, Microsoft Azurite is used to emulate Azure Table Storage.



\---



\## 2. Technologies Used



\* C#

\* .NET 8

\* Azure Functions v4

\* Azure Table Storage

\* Azure File Storage

\* Microsoft Azurite

\* Azure.Data.Tables

\* Azure.Storage.Files.Shares

\* Docker

\* Docker Hub

\* Postman

\* GitHub



\---



\## 3. Project Structure



```text

CoffeeNChillPart1/

│

├── CoffeeNChill.Functions/

│   ├── Functions/

│   │   ├── CreateMenuItem.cs

│   │   ├── GetAllMenuItems.cs

│   │   ├── GetMenuItemsByCategory.cs

│   │   ├── UpdateMenuItem.cs

│   │   ├── DeleteMenuItem.cs

│   │   ├── UploadStaffDocument.cs

│   │   ├── ListStaffDocuments.cs

│   │   └── DownloadStaffDocument.cs

│   │

│   ├── Models/

│   │   └── MenuItem.cs

│   │

│   ├── Services/

│   │   ├── MenuService.cs

│   │   └── DocumentService.cs

│   │

│   ├── Dockerfile

│   ├── .dockerignore

│   ├── Program.cs

│   ├── host.json

│   └── CoffeeNChill\_Functions.csproj

│

├── docs/

│   └── CoffeeNChill-Part1.postman\_collection.json

│

├── .gitignore

└── README.md

```



\---



\## 4. Azure Table Storage – MenuItems



The `MenuItems` table stores canteen menu information.



Each menu item contains:



\* PartitionKey – menu category

\* RowKey – unique menu item ID/SKU

\* Name

\* Description

\* Price

\* IsAvailable



Example categories include:



\* Hot Drinks

\* Cold Drinks

\* Pastries

\* Sandwiches



\---



\## 5. HTTP API Endpoints



\### Menu Management



| Method | Endpoint                        | Purpose                         |

| ------ | ------------------------------- | ------------------------------- |

| POST   | `/api/menu`                     | Create a menu item              |

| GET    | `/api/menu`                     | Retrieve all menu items         |

| GET    | `/api/menu/category/{category}` | Retrieve menu items by category |

| PUT    | `/api/menu/{category}/{id}`     | Update a menu item              |

| DELETE | `/api/menu/{category}/{id}`     | Delete a menu item              |



\### Staff Documents



| Method | Endpoint                             | Purpose                   |

| ------ | ------------------------------------ | ------------------------- |

| POST   | `/api/documents/upload`              | Upload a staff document   |

| GET    | `/api/documents`                     | List staff documents      |

| GET    | `/api/documents/download/{fileName}` | Download a staff document |



\---



\## 6. Running Locally



\### Prerequisites



Install:



\* .NET 8 SDK

\* Azure Functions Core Tools v4

\* Docker Desktop

\* Postman



\### Start Azurite



Run:



```powershell

docker run -d --name coffeenchill-azurite -p 10000:10000 -p 10001:10001 -p 10002:10002 mcr.microsoft.com/azure-storage/azurite

```



Azurite provides local emulation of Azure Blob, Queue and Table Storage.



\### Run the Functions locally



From the `CoffeeNChill.Functions` directory:



```powershell

func start

```



The API will be available at:



```text

http://localhost:7071

```



\---



\## 7. Running with Docker



\### Build the Functions image



From the `CoffeeNChill.Functions` directory:



```powershell

docker build -t coffeenchill-functions:v1.0 .

```



\### Run the Functions container



```powershell

docker run -d --name coffeenchill-functions --network coffeenchill-network -p 7071:80 coffeenchill-functions:v1.0

```



The Functions API is available at:



```text

http://localhost:7071

```



\### Check running containers



```powershell

docker ps

```



The following containers should be running:



```text

coffeenchill-functions

coffeenchill-azurite

```



\---



\## 8. Postman Testing



The complete Postman collection is included in:



```text

docs/CoffeeNChill-Part1.postman\_collection.json

```



The collection tests all eight HTTP endpoints.



The Dockerized API was successfully tested with:



\* Create Menu Item – 201 Created

\* Get All Menu Items – 200 OK

\* Get Menu Items By Category – 200 OK

\* Update Menu Item – 200 OK

\* Delete Menu Item – 200 OK

\* Upload Staff Document – 201 Created

\* List Staff Documents – 200 OK

\* Download Staff Document – 200 OK



\---



\## 9. Azure File Storage



Staff documents are stored in the Azure File Share:



```text

staff-docs

```



The application uses the Azure Storage Files SDK to upload, list and download staff documents.



The Azure Storage connection string is stored locally in `local.settings.json`.



For security, `local.settings.json` is excluded from version control and must never be committed to GitHub.



\---



\## 10. Security



Sensitive configuration values are stored in `local.settings.json`.



The file is excluded through `.gitignore`.



No Azure Storage access keys or other secrets should be committed to the GitHub repository.



\---



\## 11. GitHub Repository



The source code, Docker configuration and Postman collection are maintained in the project's GitHub repository.



\---



\## 12. Docker Hub



The Dockerized Functions application is built and tagged as:



```text

coffeenchill-functions:v1.0

```



The Azurite image is also prepared for Docker Hub publication according to the assignment requirements.



\---



\## 13. Team / Individual Contributions



\### Student



\*\*Faizaan\*\*



Responsibilities include:



\* Azure Functions development

\* Azure Table Storage implementation

\* Azure File Storage implementation

\* Docker containerization

\* Azurite configuration

\* Postman API testing

\* GitHub version control

\* Documentation



\---



\## 14. Video Demonstration



An unlisted YouTube demonstration video will be added here after recording.



\*\*YouTube:\*\* `TO BE ADDED`



The video will demonstrate:



\* Starting the required Docker containers

\* Running the CoffeeNChill Functions application

\* Testing the HTTP endpoints using Postman

\* Successful menu operations

\* Successful staff document operations



\---



\## 15. Academic Integrity



This project was developed and tested as part of the CoffeeNChill Canteen Management System Portfolio of Evidence.



All implementation decisions should be understood and explained by the student during assessment.




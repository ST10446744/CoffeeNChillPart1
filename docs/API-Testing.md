# CoffeeNChill Part 1 - API Testing

## Postman Test Results

The following HTTP endpoints were tested successfully:

1. POST /api/menu - Create Menu Item - 201 Created
2. GET /api/menu - Get All Menu Items - 200 OK
3. GET /api/menu/category/{category} - Get Menu Items By Category - 200 OK
4. PUT /api/menu/{category}/{id} - Update Menu Item - 200 OK
5. DELETE /api/menu/{category}/{id} - Delete Menu Item - 200 OK
6. POST /api/documents/upload - Upload Staff Document - 201 Created
7. GET /api/documents - List Staff Documents - 200 OK
8. GET /api/documents/download/{fileName} - Download Staff Document - 200 OK

## Postman Collection

The complete Postman collection is stored in:

docs/CoffeeNChill-Part1.postman_collection.json

## Storage

Menu items are stored in the MenuItems Azure Table.

Staff documents are stored in the staff-docs Azure File Share.

## Docker

The API was also tested while running inside the Docker Functions container.

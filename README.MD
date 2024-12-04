# Burger Mania API

An ASP.NET Core Web API for managing orders, products, users, and their relationships in a fast-food application. The API includes endpoints for creating and managing orders, associating products with orders, and linking orders with users.

## Features

- **Order Management**:
  - Create orders with multiple products.
  - Handle product quantities in orders.
  - Automatically calculate the total price for an order based on product prices and quantities.
  
- **Product Management**:
  - Query and associate products with orders.
  
- **User Management**:
  - Link users with their orders.

---

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL 16](https://www.postgresql.org/download/)


### Installation
To get the server up you should do:
1. Clone this repository

2. Install the dependencies with
```
dotnet restore
```

3. Configure the enviroment appsettings.json file to set the Database URL in DefaultConnection field to match your PostgreSQL setup.

4. Create the initial migration
```
dotnet ef migrations add initial
```

5. Update the database
```
dotnet ef database update
```

6. Run the project
```
dotnet run
```

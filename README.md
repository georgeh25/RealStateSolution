# RealStateSolution
# Technical Test Real Estate API - Jorge Home

This project is an API for a real estate company that manages properties in the United States. The API includes functionalities such as:

- Creating a new building or property.
- Adding images to properties.
- Changing the price of a property.
- Updating property information.
- Listing properties with filters.

## Features

- **.NET 8**: Utilizes the latest version of .NET.
- **JWT Authentication**: Protects endpoints using JWT token-based authentication.
- **Entity Framework Core**: Interacts with a **SQL Server** database to manage property information.
- **Hexagonal Architecture**: Designed to separate responsibilities into different layers (Domain, Application, Infrastructure, API, Tests).
- **Swagger**: Provides an interface for testing and documenting the API.

## Requirements

- **.NET 8 SDK**
- **SQL Server**

## Installation

 Clone the repository:

    ```bash
    git clone https://github.com/yourusername/realestate-api.git
    cd realestate-api
    ```

 Configure the environment variables in the `appsettings.json` file:

    ```json
    "Jwt": {
      "Key": "OneSuperSecureKeyWith32Characters",
      "Issuer": "RealEstateAPI",
      "Audience": "RealEstateUsers"
    },
    "ConnectionStrings": {
      "DefaultConnection": "Server=localhost;Database=RealEstateDb;Trusted_Connection=True;"
    }
    ```

 Apply the database migrations:

    ```bash
    dotnet ef database update --project RealEstate.Infrastructure --startup-project RealEstate.API
    ```

## Security

This project uses **JWT** for user authentication. You can generate tokens through the **login** endpoint:

POST /api/auth/login
{
  "username": "admin",
  "password": "password"
}

## Tests
The application includes two types of tests: unit and integration.  
The integration tests were implemented without JWT security verification, so the `[Authorize]` attribute in the PropertyController should be commented out.

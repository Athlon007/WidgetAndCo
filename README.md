# WidgetAndCo Store

Cloud Databases assignment for Cloud course at the InHolland University of Applied Sciences.

## Getting Started

Please setup Microsoft SQL Server and Azurite emulator before running the project! Make sure that `WidgetAndCo/local.settings.json` and `WidgetAndCo/ReviewProcessingFunction/local.settings.json` are configured correctly.

- Clone the repository
- Open the project in your IDE (IntelliJ Rider, Visual Studio Code, etc.)
- To run migrations, open the terminal and run `dotnet ef database update`
- Run "WidgetAndCo: http" (I did not test for https, but it should work)
- RECOMMENDED: Run `./utils/azurite.sh` to start the Azurite emulator for local development
- You should have Swagger UI open in your browser, if not, go to `http://localhost:5173/swagger/index.html`

## Features

- Swagger UI for API documentation
- CRUD operations for products
- CRUD operations for reviews through Azure Functions
- User authentication and authorization
- User roles: Admin, User
- User registration
- User login

## Default users

- Admin:
  - Email: 'admin@example.com'
  - Password: 'AdminPassword123' 

(Note: admin user will always be created, if no admin user is found in the database)

## Author

- [Konrad Figura](680886@student.inholland.nl)

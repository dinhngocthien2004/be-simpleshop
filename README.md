# SimpleShop Backend - ASP.NET Core Web API

## Project Structure
```
SimpleShop.API/         → Controllers, Program.cs, appsettings.json
SimpleShop.Repo/        → Models, DbContext, Repositories
SimpleShop.Service/     → Service interfaces & implementations
```

## Setup
1. Update connection string in `SimpleShop.API/appsettings.json`
2. `dotnet restore`
3. `dotnet run --project SimpleShop.API`

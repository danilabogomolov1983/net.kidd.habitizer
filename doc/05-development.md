# Development

## Prerequisites

- .NET SDK 10
- SQL Server access for local runtime work
- Docker Desktop or Docker Engine for integration tests that start containers

## Migrations

Persistence migrations are maintained in `src/Persistence`.

Set the SQL connection string:

```powershell
[Environment]::SetEnvironmentVariable("SQL_CONNECTION_STRING", "Server=tcp:localhost,1433;Database=wst-trios-core-local;User Id=sa;Password=MyP@ssword;TrustServerCertificate=True;")
```

Add a migration from the repository root:

```powershell
dotnet ef migrations add Initial `
  --project .\src\Persistence\Wst.Tools.PosiBridge.Persistence.csproj `
  --context Wst.Tools.PosiBridge.Persistence.DbContext.PortfolioDbContext
```

Apply migrations:

```powershell
dotnet ef database update `
  --project .\src\Persistence\Wst.Tools.PosiBridge.Persistence.csproj `
  --context Wst.Tools.PosiBridge.Persistence.DbContext.PortfolioDbContext
```

## Tests

Fast tests live under `test/`.

Run one test project:

```powershell
dotnet test .\test\<Domain>\<Module>\*.Test.csproj
```

Integration tests live under `test_integration/`.

Run one integration test project:

```powershell
dotnet test .\test_integration\<Domain>\<Module>\*.IntegrationTest.csproj
```

## Notes

- Shared test helpers live under `test/TestCompanion` and `test_integration/TestCompanion`
- If no test connection string is configured, some integration tests start an ephemeral SQL Server container

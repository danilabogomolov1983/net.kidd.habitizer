# PosiBridge Persistence: Migrations

## Set connection string

PowerShell:

```powershell
[Environment]::SetEnvironmentVariable("SQL_CONNECTION_STRING", "Server=tcp:localhost,1433;Database=wst-trios-core-local;User Id=sa;Password=MyP@ssword;TrustServerCertificate=True;")
```

## Get connection string

PowerShell:

```powershell
[Environment]::GetEnvironmentVariable("SQL_CONNECTION_STRING")
```

## Add a migration

Run from repository root:

```powershell
dotnet ef migrations add Initial `
  --project .\src\Persistence\Wst.Tools.PosiBridge.Persistence.csproj `
  --context Wst.Tools.PosiBridge.Persistence.DbContext.PortfolioDbContext
```

Migration files are generated in `src/Persistence/Migrations`.

## Apply migrations to database

```powershell
dotnet ef database update `
  --project .\src\Persistence\Wst.Tools.PosiBridge.Persistence.csproj `
  --context Wst.Tools.PosiBridge.Persistence.DbContext.PortfolioDbContext
```

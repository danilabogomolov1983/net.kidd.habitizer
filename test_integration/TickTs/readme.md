# Habitizer TickTs Integration Tests


## Start Tests (Default: Fake TickTs Client)

If `test_integration/TickTs/appsettings.test.json` is missing,
tests use a faked `ITickTsHttpClient`.

This fallback keeps the integration tests runnable without Tick-TS credentials
or network access.

Run from repository root:

```powershell
dotnet test .\test_integration\TickTs\Wst.Tools.Habitizer.TickTs.IntegrationTest.csproj
```

## Start Tests (Using Real Tick-TS Instead Of The Fake Client)

1. Create local file `test_integration/TickTs/appsettings.test.json`. See documentation of TickTs to get the configuration values:

```json
{
  "TickTsSettings": {
    "BaseUrl": "https://api.tick-ts.de:<port>",
    "Token": "tbmx_access_password {\"username\":\"<user>\",\"password\":\"<password>\"}",
    "ResolvedAddress": "<resolved-ip>"
  }
}
```

2. Run tests:

```powershell
dotnet test .\test_integration\TickTs\Wst.Tools.Habitizer.TickTs.IntegrationTest.csproj
```

## Troubleshooting

- Real Tick-TS mode fails: verify `TickTsSettings` values in `appsettings.test.json`.
- Changes to `appsettings.test.json` are not picked up: restart IDE/terminal and rerun tests.
- Unexpected fake responses are returned: confirm `appsettings.test.json` exists in the test project's output directory.

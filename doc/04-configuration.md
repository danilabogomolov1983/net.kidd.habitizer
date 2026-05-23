# Configuration

The daemon loads configuration from `appsettings.json` and environment-specific files such as `appsettings.Local.json`.

The template file is:

- `src/Apps/Wst.Tools.PosiBridge.SyncDaemon/appsettings.template.json`

## Required sections

### `Persistence`

Connection string for the internal PosiBridge database.

### `TradixSettings`

Connection string for the Tradix source database.

### `TickTsSettings`

- `BaseUrl`
- `Token`
- `TimeoutSeconds`
- `ResolvedAddress`

The TickTs module uses these settings to build its HTTP client and authenticate snapshot requests.

### `SyncSettings`

Maps snapshot sources to the account names that should be synchronized.

Example shape:

```json
{
  "SyncSettings": {
    "Accounts": {
      "Tradix": ["ACCOUNT_A"],
      "TickTs": ["ACCOUNT_B"]
    }
  }
}
```

## Startup validation

The application validates options on startup for:

- `SyncSettings`
- `TradixSettings`
- `TickTsSettings`

If required settings are missing or invalid, the daemon fails fast during bootstrapping.

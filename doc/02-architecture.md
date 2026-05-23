# Architecture

The solution follows a layered structure with clear responsibilities between business model, application logic, infrastructure, and source-specific adapters.

## Projects

- `src/Domain` - core entities, value objects, enums, and persistence contracts
- `src/Application` - use-case services for sources, accounts, instruments, positions, and snapshot synchronization
- `src/Persistence` - SQL Server persistence with Entity Framework Core
- `src/Tradix` - Tradix snapshot provider backed by direct database access
- `src/TickTs` - TickTs snapshot provider backed by HTTP
- `src/Shared.Kernel` - shared functional and validation helpers
- `src/Apps/Wst.Tools.PosiBridge.SyncDaemon` - hosted worker that schedules synchronization jobs

## Runtime composition

The sync daemon wires the modules together through dependency injection:

1. `AddApplication`
2. `AddPersistence`
3. `AddTradix`
4. `AddTickTs`
5. Quartz job registration

## Storage model

The internal database is managed by `PortfolioDbContext` and stores:

- `Sources`
- `Accounts`
- `Instruments`
- `Positions`

The default database schema is defined in the persistence module and migrations are generated in `src/Persistence/Migrations`.

## Extension model

New snapshot sources can be added by:

1. Extending `ESnapshotSource`
2. Implementing `IPortfolioSnapshotProvider`
3. Registering the provider as a keyed service
4. Adding configuration and a scheduled worker if needed


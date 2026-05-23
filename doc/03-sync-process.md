# Sync Process

Synchronization is driven by Quartz jobs in the sync daemon.

## Scheduled workers

- `TradixSyncWorker`
- `TickTsSyncWorker`

Each worker:

1. Checks whether accounts are configured for its source
2. Creates a DI scope
3. Resolves `Application.Snapshot.Sync.Service`
4. Starts synchronization for one `ESnapshotSource`

At startup each worker runs once immediately and then every 10 minutes at minute `5/10`.

## Snapshot flow

`Application.Snapshot.Sync.Service` coordinates the process:

1. Read configured accounts for the source
2. Resolve the source-specific `IPortfolioSnapshotProvider`
3. Fetch a normalized `Snapshot`
4. Pass the snapshot to `Application.Snapshot.Merge.Service`

## Merge behavior

The merge service keeps the internal data model aligned with the latest snapshot:

1. Ensure the `Source` exists
2. Ensure missing `Accounts` exist
3. Ensure missing `Instruments` exist
4. Delete existing positions for the source
5. Insert the new positions from the snapshot
6. Update account `LastUpdatedAt`

This means synchronization is source-based replacement, not incremental position patching.

## Source adapters

### Tradix

- Reads positions directly from the Tradix SQL database
- Filters by configured account names
- Maps database rows into the internal snapshot model

### TickTs

- Calls the TickTs HTTP endpoint
- Fetches snapshots account by account
- Merges the received positions into one internal snapshot


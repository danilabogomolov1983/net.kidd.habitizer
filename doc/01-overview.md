# Overview

`wst-tools-posibridge` is a .NET 10 solution for importing portfolio position snapshots from external sources into a central portfolio database.

## Goal

The solution normalizes positions from different providers and stores them in one internal model.

Today the active runtime entrypoint is the sync daemon:

- `src/Apps/Wst.Tools.PosiBridge.SyncDaemon`

The daemon currently synchronizes data from these sources:

- `Tradix`
- `TickTs`

## Main business objects

- `Source` - external origin of imported data
- `Account` - portfolio account within a source
- `Instrument` - traded instrument identified by ISIN
- `Position` - normalized portfolio holding
- `Snapshot` - a full position set fetched from one source

## Current scope

- Read snapshots from external systems
- Merge them into the internal persistence model
- Run synchronization on a schedule with Quartz
- Validate configuration at startup

There is a `src/Api` folder, but it is not part of the active solution file and currently does not contain application source code.


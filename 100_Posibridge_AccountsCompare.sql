declare @LeftSourceName nvarchar(255) = N'TRADIX';
declare @LeftAccountName nvarchar(255) = N'HVM_7814';
declare @RightSourceName nvarchar(255) = N'TICK_TS';
declare @RightAccountName nvarchar(255) = N'7873';

declare @LeftColumnName nvarchar(511) = @LeftSourceName + N' / ' + @LeftAccountName;
declare @RightColumnName nvarchar(511) = @RightSourceName + N' / ' + @RightAccountName;

declare @Isin nvarchar(12) = N'US1912411089';

drop table if exists #AccountDetails;
drop table if exists #LeftAccount;
drop table if exists #RightAccount;
drop table if exists #LeftPositions;
drop table if exists #RightPositions;
drop table if exists #PositionPairs;

select
    a.Id,
    a.Name,
    a.LastUpdatedAt,
    a.SourceId,
    s.Name as SourceName
into #AccountDetails
from [portfolio].[Accounts] as a
left join [portfolio].[Sources] as s
    on s.Id = a.SourceId
where
    (s.Name = @LeftSourceName and a.Name = @LeftAccountName)
    or
    (s.Name = @RightSourceName and a.Name = @RightAccountName);

select *
into #LeftAccount
from #AccountDetails
where SourceName = @LeftSourceName
  and Name = @LeftAccountName;

select *
into #RightAccount
from #AccountDetails
where SourceName = @RightSourceName
  and Name = @RightAccountName;

if not exists (select 1 from #LeftAccount)
begin
    throw 50000, 'Left account was not found in [portfolio].[Accounts] for the provided source and account name.', 1;
end;

if not exists (select 1 from #RightAccount)
begin
    throw 50001, 'Right account was not found in [portfolio].[Accounts] for the provided source and account name.', 1;
end;

select
    p.AccountId,
    p.InstrumentId,
    i.Isin,
    p.NetSize,
    p.NetValue,
    p.UnrealisedAverageCost,
    p.UnrealisedProfit,
    p.UnrealisedProfitPercent,
    p.ReferencePrice_Price,
    p.ReferencePrice_Currency,
    p.ReferencePrice_Exchange,
    p.ReferencePrice_CurrencySpot
into #LeftPositions
from [portfolio].[Positions] as p
inner join #LeftAccount as a
    on a.Id = p.AccountId
inner join [portfolio].[Instruments] as i
    on i.Id = p.InstrumentId;

select
    p.AccountId,
    p.InstrumentId,
    i.Isin,
    p.NetSize,
    p.NetValue,
    p.UnrealisedAverageCost,
    p.UnrealisedProfit,
    p.UnrealisedProfitPercent,
    p.ReferencePrice_Price,
    p.ReferencePrice_Currency,
    p.ReferencePrice_Exchange,
    p.ReferencePrice_CurrencySpot
into #RightPositions
from [portfolio].[Positions] as p
inner join #RightAccount as a
    on a.Id = p.AccountId
inner join [portfolio].[Instruments] as i
    on i.Id = p.InstrumentId;

select
    coalesce(l.Isin, r.Isin) as Isin,

    l.AccountId as LeftAccountId,
    l.InstrumentId as LeftInstrumentId,
    l.NetSize as LeftNetSize,
    l.NetValue as LeftNetValue,
    l.UnrealisedAverageCost as LeftUnrealisedAverageCost,
    l.UnrealisedProfit as LeftUnrealisedProfit,
    l.UnrealisedProfitPercent as LeftUnrealisedProfitPercent,
    l.ReferencePrice_Price as LeftReferencePrice_Price,
    l.ReferencePrice_Currency as LeftReferencePrice_Currency,
    l.ReferencePrice_Exchange as LeftReferencePrice_Exchange,
    l.ReferencePrice_CurrencySpot as LeftReferencePrice_CurrencySpot,

    r.AccountId as RightAccountId,
    r.InstrumentId as RightInstrumentId,
    r.NetSize as RightNetSize,
    r.NetValue as RightNetValue,
    r.UnrealisedAverageCost as RightUnrealisedAverageCost,
    r.UnrealisedProfit as RightUnrealisedProfit,
    r.UnrealisedProfitPercent as RightUnrealisedProfitPercent,
    r.ReferencePrice_Price as RightReferencePrice_Price,
    r.ReferencePrice_Currency as RightReferencePrice_Currency,
    r.ReferencePrice_Exchange as RightReferencePrice_Exchange,
    r.ReferencePrice_CurrencySpot as RightReferencePrice_CurrencySpot
into #PositionPairs
from #LeftPositions as l
full outer join #RightPositions as r
    on r.Isin = l.Isin;

--------------------------------------------------------------------------------
-- Raw account details
--------------------------------------------------------------------------------

--select *
--from #LeftAccount;

--select *
--from #RightAccount;

--------------------------------------------------------------------------------
-- Account comparison
--------------------------------------------------------------------------------

declare @Sql nvarchar(max);

set @Sql = N'
select
    comparison.FieldName,
    comparison.LeftValue as ' + quotename(@LeftColumnName) + N',
    comparison.RightValue as ' + quotename(@RightColumnName) + N',
    cast(case
        when isnull(comparison.LeftValue, ''<NULL>'') = isnull(comparison.RightValue, ''<NULL>'')
        then 1 else 0
    end as bit) as IsEqual
from #LeftAccount as l
cross join #RightAccount as r
cross apply
(
    values
        (''Id'', convert(nvarchar(100), l.Id), convert(nvarchar(100), r.Id)),
        (''SourceId'', convert(nvarchar(100), l.SourceId), convert(nvarchar(100), r.SourceId)),
        (''SourceName'', convert(nvarchar(100), l.SourceName), convert(nvarchar(100), r.SourceName)),
        (''Name'', convert(nvarchar(100), l.Name), convert(nvarchar(100), r.Name)),
        (''LastUpdatedAt'', convert(nvarchar(33), l.LastUpdatedAt, 126), convert(nvarchar(33), r.LastUpdatedAt, 126))
) as comparison(FieldName, LeftValue, RightValue)
order by comparison.FieldName;
';

exec sys.sp_executesql @Sql;

--------------------------------------------------------------------------------
-- Raw positions
--------------------------------------------------------------------------------

--select *
--from #LeftPositions
--order by Isin;

--select *
--from #RightPositions
--order by Isin;

--------------------------------------------------------------------------------
-- Position comparison
--------------------------------------------------------------------------------

set @Sql = N'
select
    p.Isin,
    comparison.FieldName,
    comparison.LeftValue as ' + quotename(@LeftColumnName) + N',
    comparison.RightValue as ' + quotename(@RightColumnName) + N',
    cast(case
        when isnull(comparison.LeftValue, ''<NULL>'') = isnull(comparison.RightValue, ''<NULL>'')
        then 1 else 0
    end as bit) as IsEqual
from #PositionPairs as p
cross apply
(
    values
        (''AccountId'', convert(nvarchar(100), p.LeftAccountId), convert(nvarchar(100), p.RightAccountId)),
        (''InstrumentId'', convert(nvarchar(100), p.LeftInstrumentId), convert(nvarchar(100), p.RightInstrumentId)),
        (''NetSize'', convert(nvarchar(100), p.LeftNetSize), convert(nvarchar(100), p.RightNetSize)),
        (''NetValue'', convert(nvarchar(100), p.LeftNetValue), convert(nvarchar(100), p.RightNetValue)),
        (''UnrealisedAverageCost'', convert(nvarchar(100), p.LeftUnrealisedAverageCost), convert(nvarchar(100), p.RightUnrealisedAverageCost)),
        (''UnrealisedProfit'', convert(nvarchar(100), p.LeftUnrealisedProfit), convert(nvarchar(100), p.RightUnrealisedProfit)),
        (''UnrealisedProfitPercent'', convert(nvarchar(100), p.LeftUnrealisedProfitPercent), convert(nvarchar(100), p.RightUnrealisedProfitPercent)),
        (''ReferencePrice_Price'', convert(nvarchar(100), p.LeftReferencePrice_Price), convert(nvarchar(100), p.RightReferencePrice_Price)),
        (''ReferencePrice_Currency'', convert(nvarchar(100), p.LeftReferencePrice_Currency), convert(nvarchar(100), p.RightReferencePrice_Currency)),
        (''ReferencePrice_Exchange'', convert(nvarchar(100), p.LeftReferencePrice_Exchange), convert(nvarchar(100), p.RightReferencePrice_Exchange)),
        (''ReferencePrice_CurrencySpot'', convert(nvarchar(100), p.LeftReferencePrice_CurrencySpot), convert(nvarchar(100), p.RightReferencePrice_CurrencySpot))
) as comparison(FieldName, LeftValue, RightValue)
where p.Isin = @Isin
order by p.Isin, comparison.FieldName;
';

exec sys.sp_executesql
    @Sql,
    N'@Isin nvarchar(12)',
    @Isin = @Isin;
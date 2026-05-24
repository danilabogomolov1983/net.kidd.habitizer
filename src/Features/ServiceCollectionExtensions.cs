using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AccountBulkAddMissing = Net.Kidd.Habitizer.Features.Account.Bulk.AddMissing;
using AccountBulkDeleteBySource = Net.Kidd.Habitizer.Features.Account.Bulk.DeleteBySource;
using AccountBulkGetByIds = Net.Kidd.Habitizer.Features.Account.Bulk.GetByIds;
using AccountBulkGetBySourceAndNames = Net.Kidd.Habitizer.Features.Account.Bulk.GetBySourceAndNames;
using AccountBulkGetBySources = Net.Kidd.Habitizer.Features.Account.Bulk.GetBySources;
using AccountBulkUpdate = Net.Kidd.Habitizer.Features.Account.Bulk.Update;
using AccountGet = Net.Kidd.Habitizer.Features.Account.Get;
using AccountGetList = Net.Kidd.Habitizer.Features.Account.GetList;
using AccountPost = Net.Kidd.Habitizer.Features.Account.Post;
using AccountPut = Net.Kidd.Habitizer.Features.Account.Put;
using InstrumentBulkAddMissing = Net.Kidd.Habitizer.Features.Instrument.Bulk.AddMissing;
using InstrumentBulkDeleteByIsins = Net.Kidd.Habitizer.Features.Instrument.Bulk.DeleteByIsins;
using InstrumentBulkGetByIsins = Net.Kidd.Habitizer.Features.Instrument.Bulk.GetByIsins;
using InstrumentGet = Net.Kidd.Habitizer.Features.Instrument.Get;
using InstrumentGetList = Net.Kidd.Habitizer.Features.Instrument.GetList;
using InstrumentPost = Net.Kidd.Habitizer.Features.Instrument.Post;
using PositionBulkAddMissing = Net.Kidd.Habitizer.Features.Position.Bulk.AddMissing;
using PositionBulkDeleteByAccount = Net.Kidd.Habitizer.Features.Position.Bulk.DeleteByAccount;
using PositionBulkDeleteBySource = Net.Kidd.Habitizer.Features.Position.Bulk.DeleteBySource;
using PositionBulkGetByAccounts = Net.Kidd.Habitizer.Features.Position.Bulk.GetByAccounts;
using PositionBulkGetBySource = Net.Kidd.Habitizer.Features.Position.Bulk.GetBySource;
using PositionGet = Net.Kidd.Habitizer.Features.Position.Get;
using PositionGetList = Net.Kidd.Habitizer.Features.Position.GetList;
using PositionPost = Net.Kidd.Habitizer.Features.Position.Post;
using PositionPut = Net.Kidd.Habitizer.Features.Position.Put;
using SnapshotMerge = Net.Kidd.Habitizer.Features.Snapshot.Merge;
using SnapshotSave = Net.Kidd.Habitizer.Features.Snapshot.Save;
using SnapshotSync = Net.Kidd.Habitizer.Features.Snapshot.Sync;
using SourceBulkAddMissing = Net.Kidd.Habitizer.Features.Source.Bulk.AddMissing;
using SourceBulkDeleteByNames = Net.Kidd.Habitizer.Features.Source.Bulk.DeleteByNames;
using SourceBulkGetByNames = Net.Kidd.Habitizer.Features.Source.Bulk.GetByNames;
using SourceGet = Net.Kidd.Habitizer.Features.Source.Get;
using SourceGetList = Net.Kidd.Habitizer.Features.Source.GetList;
using SourcePost = Net.Kidd.Habitizer.Features.Source.Post;
using Net.Kidd.Habitizer.Shared.Kernel.Validation;
namespace Net.Kidd.Habitizer.Features;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IValidator<Snapshot.Configuration.SyncSettings>, Snapshot.Configuration.SyncSettingsValidator>();
        services.AddOptions<Snapshot.Configuration.SyncSettings>()
            .Bind(configuration.GetSection(Snapshot.Configuration.SyncSettings.ConfigurationSection))
            .ValidateFluentValidation()
            .ValidateOnStart();

        services.AddScoped<SourcePost.Service>();
        services.AddScoped<SourceBulkAddMissing.Service>();
        services.AddScoped<SourceBulkGetByNames.Service>();
        services.AddScoped<SourceBulkDeleteByNames.Service>();
        services.AddScoped<SourceGet.Service>();
        services.AddScoped<SourceGetList.Service>();
        services.AddScoped<AccountPost.Service>();
        services.AddScoped<AccountPut.Service>();
        services.AddScoped<AccountBulkAddMissing.Service>();
        services.AddScoped<AccountBulkGetByIds.Service>();
        services.AddScoped<AccountBulkGetBySources.Service>();
        services.AddScoped<AccountBulkGetBySourceAndNames.Service>();
        services.AddScoped<Net.Kidd.Habitizer.Features.Account.Bulk.Delete.Service>();
        services.AddScoped<AccountBulkUpdate.Service>();
        services.AddScoped<AccountBulkDeleteBySource.Service>();
        services.AddScoped<AccountGet.Service>();
        services.AddScoped<AccountGetList.Service>();
        services.AddScoped<InstrumentBulkAddMissing.Service>();
        services.AddScoped<InstrumentBulkGetByIsins.Service>();
        services.AddScoped<InstrumentBulkDeleteByIsins.Service>();
        services.AddScoped<InstrumentPost.Service>();
        services.AddScoped<InstrumentGet.Service>();
        services.AddScoped<InstrumentGetList.Service>();
        services.AddScoped<PositionPost.Service>();
        services.AddScoped<PositionBulkAddMissing.Service>();
        services.AddScoped<PositionBulkGetByAccounts.Service>();
        services.AddScoped<PositionBulkGetBySource.Service>();
        services.AddScoped<PositionBulkDeleteByAccount.Service>();
        services.AddScoped<PositionBulkDeleteBySource.Service>();
        services.AddScoped<PositionPut.Service>();
        services.AddScoped<PositionGet.Service>();
        services.AddScoped<PositionGetList.Service>();

        services.AddScoped<Snapshot.PortfolioSnapshotProviderFactory>();
        services.AddScoped<SnapshotMerge.Service>();
        services.AddScoped<SnapshotSave.Service>();
        services.AddScoped<SnapshotSync.Service>();
        
        services.AddSingleton(TimeProvider.System);
    }
}

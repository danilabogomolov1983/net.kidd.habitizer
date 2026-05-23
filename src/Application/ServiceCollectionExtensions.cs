using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AccountBulkAddMissing = Net.Kidd.Habitizer.Application.Account.Bulk.AddMissing;
using AccountBulkDeleteBySource = Net.Kidd.Habitizer.Application.Account.Bulk.DeleteBySource;
using AccountBulkGetByIds = Net.Kidd.Habitizer.Application.Account.Bulk.GetByIds;
using AccountBulkGetBySourceAndNames = Net.Kidd.Habitizer.Application.Account.Bulk.GetBySourceAndNames;
using AccountBulkGetBySources = Net.Kidd.Habitizer.Application.Account.Bulk.GetBySources;
using AccountBulkUpdate = Net.Kidd.Habitizer.Application.Account.Bulk.Update;
using AccountGet = Net.Kidd.Habitizer.Application.Account.Get;
using AccountGetList = Net.Kidd.Habitizer.Application.Account.GetList;
using AccountPost = Net.Kidd.Habitizer.Application.Account.Post;
using AccountPut = Net.Kidd.Habitizer.Application.Account.Put;
using InstrumentBulkAddMissing = Net.Kidd.Habitizer.Application.Instrument.Bulk.AddMissing;
using InstrumentBulkDeleteByIsins = Net.Kidd.Habitizer.Application.Instrument.Bulk.DeleteByIsins;
using InstrumentBulkGetByIsins = Net.Kidd.Habitizer.Application.Instrument.Bulk.GetByIsins;
using InstrumentGet = Net.Kidd.Habitizer.Application.Instrument.Get;
using InstrumentGetList = Net.Kidd.Habitizer.Application.Instrument.GetList;
using InstrumentPost = Net.Kidd.Habitizer.Application.Instrument.Post;
using PositionBulkAddMissing = Net.Kidd.Habitizer.Application.Position.Bulk.AddMissing;
using PositionBulkDeleteByAccount = Net.Kidd.Habitizer.Application.Position.Bulk.DeleteByAccount;
using PositionBulkDeleteBySource = Net.Kidd.Habitizer.Application.Position.Bulk.DeleteBySource;
using PositionBulkGetByAccounts = Net.Kidd.Habitizer.Application.Position.Bulk.GetByAccounts;
using PositionBulkGetBySource = Net.Kidd.Habitizer.Application.Position.Bulk.GetBySource;
using PositionGet = Net.Kidd.Habitizer.Application.Position.Get;
using PositionGetList = Net.Kidd.Habitizer.Application.Position.GetList;
using PositionPost = Net.Kidd.Habitizer.Application.Position.Post;
using PositionPut = Net.Kidd.Habitizer.Application.Position.Put;
using SnapshotMerge = Net.Kidd.Habitizer.Application.Snapshot.Merge;
using SnapshotSave = Net.Kidd.Habitizer.Application.Snapshot.Save;
using SnapshotSync = Net.Kidd.Habitizer.Application.Snapshot.Sync;
using SourceBulkAddMissing = Net.Kidd.Habitizer.Application.Source.Bulk.AddMissing;
using SourceBulkDeleteByNames = Net.Kidd.Habitizer.Application.Source.Bulk.DeleteByNames;
using SourceBulkGetByNames = Net.Kidd.Habitizer.Application.Source.Bulk.GetByNames;
using SourceGet = Net.Kidd.Habitizer.Application.Source.Get;
using SourceGetList = Net.Kidd.Habitizer.Application.Source.GetList;
using SourcePost = Net.Kidd.Habitizer.Application.Source.Post;
using Net.Kidd.Habitizer.Shared.Kernel.Validation;
namespace Net.Kidd.Habitizer.Application;

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
        services.AddScoped<Net.Kidd.Habitizer.Application.Account.Bulk.Delete.Service>();
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

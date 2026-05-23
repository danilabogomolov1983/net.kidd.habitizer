using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AccountBulkAddMissing = Wst.Tools.PosiBridge.Application.Account.Bulk.AddMissing;
using AccountBulkDeleteBySource = Wst.Tools.PosiBridge.Application.Account.Bulk.DeleteBySource;
using AccountBulkGetByIds = Wst.Tools.PosiBridge.Application.Account.Bulk.GetByIds;
using AccountBulkGetBySourceAndNames = Wst.Tools.PosiBridge.Application.Account.Bulk.GetBySourceAndNames;
using AccountBulkGetBySources = Wst.Tools.PosiBridge.Application.Account.Bulk.GetBySources;
using AccountBulkUpdate = Wst.Tools.PosiBridge.Application.Account.Bulk.Update;
using AccountGet = Wst.Tools.PosiBridge.Application.Account.Get;
using AccountGetList = Wst.Tools.PosiBridge.Application.Account.GetList;
using AccountPost = Wst.Tools.PosiBridge.Application.Account.Post;
using AccountPut = Wst.Tools.PosiBridge.Application.Account.Put;
using InstrumentBulkAddMissing = Wst.Tools.PosiBridge.Application.Instrument.Bulk.AddMissing;
using InstrumentBulkDeleteByIsins = Wst.Tools.PosiBridge.Application.Instrument.Bulk.DeleteByIsins;
using InstrumentBulkGetByIsins = Wst.Tools.PosiBridge.Application.Instrument.Bulk.GetByIsins;
using InstrumentGet = Wst.Tools.PosiBridge.Application.Instrument.Get;
using InstrumentGetList = Wst.Tools.PosiBridge.Application.Instrument.GetList;
using InstrumentPost = Wst.Tools.PosiBridge.Application.Instrument.Post;
using PositionBulkAddMissing = Wst.Tools.PosiBridge.Application.Position.Bulk.AddMissing;
using PositionBulkDeleteByAccount = Wst.Tools.PosiBridge.Application.Position.Bulk.DeleteByAccount;
using PositionBulkDeleteBySource = Wst.Tools.PosiBridge.Application.Position.Bulk.DeleteBySource;
using PositionBulkGetByAccounts = Wst.Tools.PosiBridge.Application.Position.Bulk.GetByAccounts;
using PositionBulkGetBySource = Wst.Tools.PosiBridge.Application.Position.Bulk.GetBySource;
using PositionGet = Wst.Tools.PosiBridge.Application.Position.Get;
using PositionGetList = Wst.Tools.PosiBridge.Application.Position.GetList;
using PositionPost = Wst.Tools.PosiBridge.Application.Position.Post;
using PositionPut = Wst.Tools.PosiBridge.Application.Position.Put;
using SnapshotMerge = Wst.Tools.PosiBridge.Application.Snapshot.Merge;
using SnapshotSave = Wst.Tools.PosiBridge.Application.Snapshot.Save;
using SnapshotSync = Wst.Tools.PosiBridge.Application.Snapshot.Sync;
using SourceBulkAddMissing = Wst.Tools.PosiBridge.Application.Source.Bulk.AddMissing;
using SourceBulkDeleteByNames = Wst.Tools.PosiBridge.Application.Source.Bulk.DeleteByNames;
using SourceBulkGetByNames = Wst.Tools.PosiBridge.Application.Source.Bulk.GetByNames;
using SourceGet = Wst.Tools.PosiBridge.Application.Source.Get;
using SourceGetList = Wst.Tools.PosiBridge.Application.Source.GetList;
using SourcePost = Wst.Tools.PosiBridge.Application.Source.Post;
using Wst.Tools.PosiBridge.Shared.Kernel.Validation;
namespace Wst.Tools.PosiBridge.Application;

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
        services.AddScoped<Wst.Tools.PosiBridge.Application.Account.Bulk.Delete.Service>();
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

using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Wst.Tools.PosiBridge.Shared.Kernel.Functional;

namespace Wst.Tools.PosiBridge.Shared.Kernel;

public class FirstLevelCache<T>(ILogger<FirstLevelCache<T>> logger) : IDisposable where T : IDisposable
{
    private record CacheItem(T Value, Timer Timer);
    

    private readonly ConcurrentDictionary<string, CacheItem> _items = new();

    private Func<ConcurrentDictionary<string, CacheItem>, ILogger<FirstLevelCache<T>>, TimeSpan, string , Timer> NewTimerInstance
        => (items, l, ttl, key) 
        => new Timer(_ =>
    {
        if (!items.TryRemove(key, out var removed))
        {
            l.LogInformation(
                "[Module:Selenium.FirstLevelCache][Step:Expire] Cache item was already removed for key {CacheKey}",
                key);
            return;
        }

        removed.Value.Dispose();
        removed.Timer.Dispose();
        l.LogInformation(
            "[Module:Selenium.FirstLevelCache][Step:Expire] Disposed cached instance for key {CacheKey} after {Ttl}",
            key,
            ttl);
    }, null, ttl, Timeout.InfiniteTimeSpan);

    private Func<TimeSpan, string, Timer> GetTimerFunc => (ttl, key) => NewTimerInstance.Applying(_items).Applying(logger).Applying(ttl).Applying(key).Invoke();
    private Func<Func<T>, TimeSpan, ILogger<FirstLevelCache<T>>, string, CacheItem> NewItem => (factory, ttl, l, key) =>
    {
        l.LogInformation(
            "[Module:Selenium.FirstLevelCache][Step:Miss] Cache miss for key {CacheKey}. Creating new instance with {Ttl}",
            key,
            ttl);

        var getTimer = GetTimerFunc.Applying(ttl).Applying(key);
        return new CacheItem(factory(), getTimer());
    };
    
    public T GetOrCreate(string key, Func<T> factory, TimeSpan ttl)
    {
        var createNewItem = NewItem.Applying(factory).Applying(ttl).Applying(logger);
        var item = _items.AddOrUpdate(
            key,
            k => createNewItem(k),
            (k, existingItem) =>
            {
                existingItem.Timer.Change(ttl, Timeout.InfiniteTimeSpan);
                return existingItem;
            });

        return item.Value;
    }

    public void Dispose()
    {
        foreach (var kv in _items)
        {
            kv.Value.Timer.Dispose();
            kv.Value.Value.Dispose();
        }

        _items.Clear();
        logger.LogInformation("[Module:Selenium.FirstLevelCache][Step:Dispose] First-level cache disposed");
    }
}


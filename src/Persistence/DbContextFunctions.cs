using Microsoft.EntityFrameworkCore;

namespace Net.Kidd.Habitizer.Persistence;

public static class DbContextFunctions
{
    public static async Task<T> UsingContext<T>(IPortfolioDbContextFactory dbContextFactory, Func<DbContext.PortfolioDbContext, Task<T>> func, int tries = 3)
    {
        try
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync();
            var result = await func(dbContext);
            return result;
        }
        catch (DbUpdateException)
        {
            if (tries > 0)
            {
                return await UsingContext(dbContextFactory, func, tries - 1);
            }

            throw;
        }
    }
}

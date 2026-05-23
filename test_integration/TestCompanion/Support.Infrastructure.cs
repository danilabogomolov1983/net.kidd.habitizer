using Testcontainers.MsSql;

namespace Net.Kidd.Habitizer.TestCompanion;

public static partial class Support
{
    public static partial class Infrastructure
    {
        public static MsSqlContainer UseContainer() =>
            new MsSqlBuilder("mcr.microsoft.com/mssql/server:2025-latest")
                .WithName("wst_tools_tests")
                .WithPassword("MyP@ssword")
                .WithCleanUp(true)
                .Build();

    }
}


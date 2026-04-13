using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace DMS.EntityFrameworkCore;

public static class DMSDbContextConfigurer
{
    public static void Configure(DbContextOptionsBuilder<DMSDbContext> builder, string connectionString)
    {
        builder.UseSqlServer(connectionString);
    }

    public static void Configure(DbContextOptionsBuilder<DMSDbContext> builder, DbConnection connection)
    {
        builder.UseSqlServer(connection);
    }
}

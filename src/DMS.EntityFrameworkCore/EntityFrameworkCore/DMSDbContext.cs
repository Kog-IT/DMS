using Abp.Zero.EntityFrameworkCore;
using DMS.Authorization.Roles;
using DMS.Authorization.Users;
using DMS.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace DMS.EntityFrameworkCore;

public class DMSDbContext : AbpZeroDbContext<Tenant, Role, User, DMSDbContext>
{
    /* Define a DbSet for each entity of the application */

    public DMSDbContext(DbContextOptions<DMSDbContext> options)
        : base(options)
    {
    }
}

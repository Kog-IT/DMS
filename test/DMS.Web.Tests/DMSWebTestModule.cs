using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using DMS.EntityFrameworkCore;
using DMS.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace DMS.Web.Tests;

[DependsOn(
    typeof(DMSWebMvcModule),
    typeof(AbpAspNetCoreTestBaseModule)
)]
public class DMSWebTestModule : AbpModule
{
    public DMSWebTestModule(DMSEntityFrameworkModule abpProjectNameEntityFrameworkModule)
    {
        abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
    }

    public override void PreInitialize()
    {
        Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(DMSWebTestModule).GetAssembly());
    }

    public override void PostInitialize()
    {
        IocManager.Resolve<ApplicationPartManager>()
            .AddApplicationPartsIfNotAddedBefore(typeof(DMSWebMvcModule).Assembly);
    }
}
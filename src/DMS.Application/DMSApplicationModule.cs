using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using DMS.Authorization;
using DMS.Customers.Jobs;

namespace DMS;

[DependsOn(
    typeof(DMSCoreModule),
    typeof(AbpAutoMapperModule))]
public class DMSApplicationModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.Authorization.Providers.Add<DMSAuthorizationProvider>();
    }

    public override void Initialize()
    {
        var thisAssembly = typeof(DMSApplicationModule).GetAssembly();

        IocManager.RegisterAssemblyByConvention(thisAssembly);

        Configuration.Modules.AbpAutoMapper().Configurators.Add(
            // Scan the assembly for classes which inherit from AutoMapper.Profile
            cfg => cfg.AddMaps(thisAssembly)
        );
    }

    public override void PostInitialize()
    {
        IocManager.Resolve<ClassifyCustomersScheduler>().Start();
    }
}

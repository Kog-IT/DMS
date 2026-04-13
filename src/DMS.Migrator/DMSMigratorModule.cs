using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using DMS.Configuration;
using DMS.EntityFrameworkCore;
using DMS.Migrator.DependencyInjection;
using Castle.MicroKernel.Registration;
using Microsoft.Extensions.Configuration;

namespace DMS.Migrator;

[DependsOn(typeof(DMSEntityFrameworkModule))]
public class DMSMigratorModule : AbpModule
{
    private readonly IConfigurationRoot _appConfiguration;

    public DMSMigratorModule(DMSEntityFrameworkModule abpProjectNameEntityFrameworkModule)
    {
        abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

        _appConfiguration = AppConfigurations.Get(
            typeof(DMSMigratorModule).GetAssembly().GetDirectoryPathOrNull()
        );
    }

    public override void PreInitialize()
    {
        Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
            DMSConsts.ConnectionStringName
        );

        Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        Configuration.ReplaceService(
            typeof(IEventBus),
            () => IocManager.IocContainer.Register(
                Component.For<IEventBus>().Instance(NullEventBus.Instance)
            )
        );
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(DMSMigratorModule).GetAssembly());
        ServiceCollectionRegistrar.Register(IocManager);
    }
}

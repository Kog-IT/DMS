using Abp.AutoMapper;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Modules;
using Abp.Net.Mail;
using Abp.TestBase;
using Abp.Zero.Configuration;
using Abp.Zero.EntityFrameworkCore;
using DMS.EntityFrameworkCore;
using DMS.Payments.Pdf;
using DMS.Payments.Storage;
using DMS.Tests.DependencyInjection;
using Castle.MicroKernel.Registration;
using NSubstitute;
using System;

namespace DMS.Tests;

[DependsOn(
    typeof(DMSApplicationModule),
    typeof(DMSEntityFrameworkModule),
    typeof(AbpTestBaseModule)
    )]
public class DMSTestModule : AbpModule
{
    public DMSTestModule(DMSEntityFrameworkModule abpProjectNameEntityFrameworkModule)
    {
        abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        abpProjectNameEntityFrameworkModule.SkipDbSeed = true;
    }

    public override void PreInitialize()
    {
        Configuration.UnitOfWork.Timeout = TimeSpan.FromMinutes(30);
        Configuration.UnitOfWork.IsTransactional = false;

        // Disable static mapper usage since it breaks unit tests (see https://github.com/aspnetboilerplate/aspnetboilerplate/issues/2052)
        Configuration.Modules.AbpAutoMapper().UseStaticMapper = false;

        Configuration.BackgroundJobs.IsJobExecutionEnabled = false;

        // Use database for language management
        Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

        RegisterFakeService<AbpZeroDbMigrator<DMSDbContext>>();

        Configuration.ReplaceService<IEmailSender, NullEmailSender>(DependencyLifeStyle.Transient);

        Configuration.ReplaceService<IReceiptPdfService>(() =>
            IocManager.IocContainer.Register(
                Component.For<IReceiptPdfService>()
                    .UsingFactoryMethod(() => Substitute.For<IReceiptPdfService>())
                    .LifestyleSingleton()
            ));

        Configuration.ReplaceService<IReceiptFileStore>(() =>
            IocManager.IocContainer.Register(
                Component.For<IReceiptFileStore>()
                    .UsingFactoryMethod(() => Substitute.For<IReceiptFileStore>())
                    .LifestyleSingleton()
            ));
    }

    public override void Initialize()
    {
        ServiceCollectionRegistrar.Register(IocManager);
    }

    private void RegisterFakeService<TService>() where TService : class
    {
        IocManager.IocContainer.Register(
            Component.For<TService>()
                .UsingFactoryMethod(() => Substitute.For<TService>())
                .LifestyleSingleton()
        );
    }
}

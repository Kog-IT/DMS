using System;
using System.Threading.Tasks;
using Abp.BackgroundJobs;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Threading;
using DMS.MultiTenancy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace DMS.Customers.Jobs;

public class ClassifyCustomersScheduler : ITransientDependency
{
    private readonly IBackgroundJobManager _jobManager;
    private readonly ISettingManager _settingManager;
    private readonly IRepository<Tenant, int> _tenantRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public ILogger<ClassifyCustomersScheduler> Logger { get; set; }

    public ClassifyCustomersScheduler(
        IBackgroundJobManager jobManager,
        ISettingManager settingManager,
        IRepository<Tenant, int> tenantRepository,
        IUnitOfWorkManager unitOfWorkManager)
    {
        _jobManager = jobManager;
        _settingManager = settingManager;
        _tenantRepository = tenantRepository;
        _unitOfWorkManager = unitOfWorkManager;
        Logger = NullLogger<ClassifyCustomersScheduler>.Instance;
    }

    public void Start()
    {
        try
        {
            AsyncHelper.RunSync(StartAsync);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to schedule customer classification jobs on startup.");
        }
    }

    private async Task StartAsync()
    {
        var frequency = await _settingManager.GetSettingValueAsync(ClassificationSettingNames.ScheduleFrequency);
        var delay = GetDelay(frequency);

        using (var uow = _unitOfWorkManager.Begin())
        {
            var tenants = _tenantRepository.GetAllList(t => t.IsActive);
            foreach (var tenant in tenants)
            {
                await _jobManager.EnqueueAsync<ClassifyCustomersJob, int>(tenant.Id, delay: delay);
            }
            await uow.CompleteAsync();
        }
    }

    private static TimeSpan GetDelay(string frequency) => frequency switch
    {
        "Daily" => TimeSpan.FromHours(24),
        "Monthly" => TimeSpan.FromDays(30),
        _ => TimeSpan.FromDays(7)
    };
}

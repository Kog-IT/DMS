using System;
using System.Threading.Tasks;
using Abp.BackgroundJobs;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Threading;

namespace DMS.Customers.Jobs;

public class ClassifyCustomersJob : BackgroundJob<int>, ITransientDependency
{
    private readonly IClassifyCustomersAppService _classifyService;
    private readonly IBackgroundJobManager _jobManager;
    private readonly ISettingManager _settingManager;

    public ClassifyCustomersJob(
        IClassifyCustomersAppService classifyService,
        IBackgroundJobManager jobManager,
        ISettingManager settingManager)
    {
        _classifyService = classifyService;
        _jobManager = jobManager;
        _settingManager = settingManager;
    }

    public override void Execute(int tenantId)
    {
        AsyncHelper.RunSync(() => ExecuteAsync(tenantId));
    }

    private async Task ExecuteAsync(int tenantId)
    {
        using (CurrentUnitOfWork.SetTenantId(tenantId))
        {
            await _classifyService.ClassifyAllAsync();
        }

        var frequency = await _settingManager.GetSettingValueAsync(ClassificationSettingNames.ScheduleFrequency);
        var delay = GetDelay(frequency);
        await _jobManager.EnqueueAsync<ClassifyCustomersJob, int>(tenantId, delay: delay);
    }

    private static TimeSpan GetDelay(string frequency) => frequency switch
    {
        "Daily" => TimeSpan.FromHours(24),
        "Monthly" => TimeSpan.FromDays(30),
        _ => TimeSpan.FromDays(7)
    };
}

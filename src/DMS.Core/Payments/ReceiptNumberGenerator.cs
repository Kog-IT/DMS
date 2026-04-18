using Abp.Configuration;
using Abp.Dependency;
using System;
using System.Threading.Tasks;

namespace DMS.Payments;

public class ReceiptNumberGenerator : ITransientDependency
{
    private readonly ISettingManager _settingManager;

    public ReceiptNumberGenerator(ISettingManager settingManager)
    {
        _settingManager = settingManager;
    }

    public async Task<string> GenerateAsync(int tenantId)
    {
        var currentYear = DateTime.UtcNow.Year;

        var storedYearStr = await _settingManager.GetSettingValueForTenantAsync(
            ReceiptSettingNames.CurrentYear, tenantId);
        var storedYear = int.TryParse(storedYearStr, out var y) ? y : 0;

        var seqStr = await _settingManager.GetSettingValueForTenantAsync(
            ReceiptSettingNames.NextSequenceNumber, tenantId);
        var seq = int.TryParse(seqStr, out var s) ? s : 1;

        if (storedYear != currentYear)
        {
            seq = 1;
            await _settingManager.ChangeSettingForTenantAsync(
                tenantId, ReceiptSettingNames.CurrentYear, currentYear.ToString());
        }

        var number = $"RCP-{currentYear}-{seq:D5}";

        await _settingManager.ChangeSettingForTenantAsync(
            tenantId, ReceiptSettingNames.NextSequenceNumber, (seq + 1).ToString());

        return number;
    }
}

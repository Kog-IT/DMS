using Abp.Configuration;
using Abp.Dependency;
using System.Threading.Tasks;

namespace DMS.Invoices;

public class InvoiceNumberGenerator : ITransientDependency
{
    private readonly ISettingManager _settingManager;

    public InvoiceNumberGenerator(ISettingManager settingManager)
    {
        _settingManager = settingManager;
    }

    public async Task<string> GenerateAsync(int tenantId)
    {
        var prefix = await _settingManager.GetSettingValueForTenantAsync(
            InvoiceSettingNames.NumberPrefix, tenantId);

        var seqStr = await _settingManager.GetSettingValueForTenantAsync(
            InvoiceSettingNames.NextSequenceNumber, tenantId);

        var seq = int.TryParse(seqStr, out var parsed) ? parsed : 1;
        var number = $"{prefix}-{seq:D4}";

        await _settingManager.ChangeSettingForTenantAsync(
            tenantId,
            InvoiceSettingNames.NextSequenceNumber,
            (seq + 1).ToString());

        return number;
    }
}

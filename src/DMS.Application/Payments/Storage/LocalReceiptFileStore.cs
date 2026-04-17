using Abp.Dependency;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace DMS.Payments.Storage;

public class LocalReceiptFileStore : IReceiptFileStore, ITransientDependency
{
    private readonly string _basePath;

    public LocalReceiptFileStore(IConfiguration configuration)
    {
        _basePath = configuration["App:ReceiptStoragePath"] ?? Path.Combine(Path.GetTempPath(), "DmsReceipts");
    }

    public async Task SaveAsync(int tenantId, string receiptNumber, byte[] content)
    {
        var dir = Path.Combine(_basePath, tenantId.ToString());
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, $"{receiptNumber}.pdf");
        await File.WriteAllBytesAsync(path, content);
    }

    public async Task<byte[]?> LoadAsync(int tenantId, string receiptNumber)
    {
        var path = Path.Combine(_basePath, tenantId.ToString(), $"{receiptNumber}.pdf");
        if (!File.Exists(path))
            return null;
        return await File.ReadAllBytesAsync(path);
    }
}

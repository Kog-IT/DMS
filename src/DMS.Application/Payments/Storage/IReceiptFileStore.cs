using System.Threading.Tasks;

namespace DMS.Payments.Storage;

public interface IReceiptFileStore
{
    Task SaveAsync(int tenantId, string receiptNumber, byte[] content);
    Task<byte[]?> LoadAsync(int tenantId, string receiptNumber);
}

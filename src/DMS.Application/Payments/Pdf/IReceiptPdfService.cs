namespace DMS.Payments.Pdf;

public interface IReceiptPdfService
{
    byte[] Generate(ReceiptPdfModel model);
}

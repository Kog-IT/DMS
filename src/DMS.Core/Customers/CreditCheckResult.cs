namespace DMS.Customers;

public class CreditCheckResult
{
    public bool IsOverLimit { get; set; }
    public decimal CreditLimit { get; set; }
    public decimal OutstandingBalance { get; set; }
    public decimal AvailableCredit { get; set; }
    public decimal UtilizationPercent { get; set; }
}

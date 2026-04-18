namespace DMS.Customers.Dto;

public class CreditStatusDto
{
    public int CustomerId { get; set; }
    public bool CreditEnabled { get; set; }
    public decimal CreditLimit { get; set; }
    public decimal OutstandingBalance { get; set; }
    public decimal AvailableCredit { get; set; }
}

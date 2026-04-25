namespace DMS.Customers.Dto;

public class CreditStatusDto
{
    public int CustomerId { get; set; }
    public bool CreditEnabled { get; set; }
    public bool IsBlocked { get; set; }
    public int CreditDays { get; set; }
    public decimal CreditLimit { get; set; }
    public decimal OutstandingBalance { get; set; }
    public decimal AvailableCredit { get; set; }
    public decimal UtilizationPercent { get; set; }
}

public class UpdateCreditLimitDto
{
    public decimal CreditLimit { get; set; }
    public bool CreditEnabled { get; set; }
    public int CreditDays { get; set; }
}

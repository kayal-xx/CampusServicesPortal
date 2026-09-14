namespace CampusServicesPortal.Api.DTOs.Fee;

public class CreateFeePaymentDto
{
    public int StudentId { get; set; }

    public string FeeType { get; set; } = string.Empty;

    public decimal Amount { get; set; }
}
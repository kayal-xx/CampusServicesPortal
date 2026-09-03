// FeePayment.cs
namespace CampusServicesPortal.Api.Entities;

public class FeePayment
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public string FeeType { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public bool IsPaid { get; set; }

    public DateTime? PaidAt { get; set; }

    public Student? Student { get; set; }
}
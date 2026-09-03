// ComplaintCategory.cs
namespace CampusServicesPortal.Api.Entities;

public class ComplaintCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
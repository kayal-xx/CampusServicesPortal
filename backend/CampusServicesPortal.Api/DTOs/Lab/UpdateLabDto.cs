using System.ComponentModel.DataAnnotations;

namespace CampusServicesPortal.Api.DTOs.Lab;

public class UpdateLabDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Location { get; set; } = string.Empty;

    [Range(1, 1000)]
    public int Capacity { get; set; }

    public bool IsActive { get; set; } = true;
}

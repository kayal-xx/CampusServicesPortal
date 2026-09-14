using System.ComponentModel.DataAnnotations;

namespace CampusServicesPortal.Api.DTOs.Hostel;

public class CreateHostelDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Location { get; set; } = string.Empty;
}
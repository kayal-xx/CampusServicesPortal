using System.ComponentModel.DataAnnotations;

namespace CampusServicesPortal.Api.DTOs.Hostel;

public class CreateHostelApplicationDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int HostelId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Semester { get; set; } = string.Empty;

    [MaxLength(500)]
    public string SpecialRequirements { get; set; }
        = string.Empty;
}
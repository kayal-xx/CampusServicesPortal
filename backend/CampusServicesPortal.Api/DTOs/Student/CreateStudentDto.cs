using System.ComponentModel.DataAnnotations;

namespace CampusServicesPortal.Api.DTOs.Student;

public class CreateStudentDto
{
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string IndexNumber { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Faculty { get; set; } = string.Empty;

    [Required]
    [Phone]
    [MaxLength(20)]
    public string ContactNumber { get; set; } = string.Empty;
}
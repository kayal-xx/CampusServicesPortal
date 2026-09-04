using CampusServicesPortal.Api.DTOs.Student;
using CampusServicesPortal.Api.Entities;
using CampusServicesPortal.Api.Repositories;

namespace CampusServicesPortal.Api.Services;

public class StudentService
{
    private readonly StudentRepository _studentRepository;

    public StudentService(StudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    // Get one student profile
    public async Task<StudentDto> GetByIdAsync(int id)
    {
        var student = await _studentRepository.GetByIdAsync(id);

        if (student is null)
        {
            throw new KeyNotFoundException("Student not found.");
        }

        return MapToDto(student);
    }

    // Search and filter students
    public async Task<List<StudentDto>> SearchAsync(
        string? search,
        string? faculty)
    {
        var students = await _studentRepository.SearchAsync(
            search,
            faculty
        );

        return students
            .Select(MapToDto)
            .ToList();
    }

    // Update student profile
    public async Task<StudentDto> UpdateAsync(
        int id,
        UpdateStudentDto request)
    {
        var student = await _studentRepository.GetByIdAsync(id);

        if (student is null)
        {
            throw new KeyNotFoundException("Student not found.");
        }

        var emailAlreadyExists =
            await _studentRepository.EmailExistsForOtherStudentAsync(
                request.Email,
                id
            );

        if (emailAlreadyExists)
        {
            throw new InvalidOperationException(
                "This email address is already used by another student."
            );
        }

        student.FullName = request.FullName.Trim();
        student.Email = request.Email.Trim().ToLower();
        student.Faculty = request.Faculty.Trim();
        student.ContactNumber = request.ContactNumber.Trim();

        await _studentRepository.UpdateAsync(student);

        return MapToDto(student);
    }

    // Deactivate student account
    public async Task DeactivateAsync(int id)
    {
        var deactivated =
            await _studentRepository.DeactivateAsync(id);

        if (!deactivated)
        {
            throw new KeyNotFoundException("Student not found.");
        }
    }

    // Convert Student entity to StudentDto
    private static StudentDto MapToDto(Student student)
    {
        return new StudentDto
        {
            Id = student.Id,
            FullName = student.FullName,
            IndexNumber = student.IndexNumber,
            Email = student.Email,
            Faculty = student.Faculty,
            ContactNumber = student.ContactNumber,
            Role = student.Role,
            IsActive = student.IsActive,
            CreatedAt = student.CreatedAt
        };
    }
}
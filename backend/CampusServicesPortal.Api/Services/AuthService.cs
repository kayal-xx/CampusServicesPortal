using CampusServicesPortal.Api.DTOs.Auth;
using CampusServicesPortal.Api.Entities;
using CampusServicesPortal.Api.Repositories;
using CampusServicesPortal.Api.Security;
using Microsoft.AspNetCore.Identity;

namespace CampusServicesPortal.Api.Services;

public class AuthService
{
    private readonly StudentRepository _studentRepository;
    private readonly JwtTokenService _jwtTokenService;
    private readonly PasswordHasher<Student> _passwordHasher;

    public AuthService(
        StudentRepository studentRepository,
        JwtTokenService jwtTokenService)
    {
        _studentRepository = studentRepository;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = new PasswordHasher<Student>();
    }

    public async Task<AuthResponseDto> RegisterAsync(
        RegisterRequestDto request)
    {
        var emailExists =
            await _studentRepository.EmailExistsAsync(request.Email);

        if (emailExists)
        {
            throw new InvalidOperationException(
                "This email address is already registered."
            );
        }

        var indexNumberExists =
            await _studentRepository.IndexNumberExistsAsync(
                request.IndexNumber
            );

        if (indexNumberExists)
        {
            throw new InvalidOperationException(
                "This index number is already registered."
            );
        }

        var student = new Student
        {
            FullName = request.FullName.Trim(),
            IndexNumber = request.IndexNumber.Trim().ToUpper(),
            Email = request.Email.Trim().ToLower(),
            Faculty = request.Faculty.Trim(),
            ContactNumber = request.ContactNumber.Trim(),
            Role = "Student",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        student.PasswordHash = _passwordHasher.HashPassword(
            student,
            request.Password
        );

        await _studentRepository.AddAsync(student);

        return CreateAuthResponse(student);
    }

    public async Task<AuthResponseDto> LoginAsync(
        LoginRequestDto request)
    {
        var student = await _studentRepository.GetByEmailAsync(
            request.Email
        );

        if (student is null)
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password."
            );
        }

        if (!student.IsActive)
        {
            throw new UnauthorizedAccessException(
                "This account has been deactivated."
            );
        }

        var passwordResult =
            _passwordHasher.VerifyHashedPassword(
                student,
                student.PasswordHash,
                request.Password
            );

        if (passwordResult == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password."
            );
        }

        return CreateAuthResponse(student);
    }

    private AuthResponseDto CreateAuthResponse(Student student)
    {
        var tokenResult = _jwtTokenService.GenerateToken(student);

        return new AuthResponseDto
        {
            Token = tokenResult.Token,
            ExpiresAt = tokenResult.ExpiresAt,
            StudentId = student.Id,
            FullName = student.FullName,
            Email = student.Email,
            Role = student.Role
        };
    }
}
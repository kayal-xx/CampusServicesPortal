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
    private readonly EmailService _emailService;

    public AuthService(
        StudentRepository studentRepository,
        JwtTokenService jwtTokenService,
        EmailService emailService)
    {
        _studentRepository = studentRepository;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = new PasswordHasher<Student>();
        _emailService = emailService;   
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

    public async Task ForgotPasswordAsync(
    ForgotPasswordRequestDto request)
    {
        var student = await _studentRepository.GetByEmailAsync(
            request.Email.Trim().ToLower()
        );

        if (student is null || !student.IsActive)
        {
            return;
        }

        var otp = Random.Shared.Next(100000, 1000000).ToString();

        var resetToken = new PasswordResetToken
        {
            StudentId = student.Id,
            Email = student.Email,
            Otp = otp,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };

        await _studentRepository.AddPasswordResetTokenAsync(resetToken);

        await _emailService.SendEmailAsync(
            student.Email,
            "Password Reset Verification Code",
            $"""
        Hello {student.FullName},

        Your Campus Services Portal password reset verification code is:

        {otp}

        This code will expire in 10 minutes.

        If you did not request a password reset, please ignore this email.

        Campus Services Portal
        """
        );
    }

    public async Task<bool> VerifyOtpAsync(
    VerifyOtpRequestDto request)
    {
        var token = await _studentRepository
            .GetLatestPasswordResetTokenAsync(
                request.Email.Trim().ToLower()
            );

        if (token is null)
        {
            return false;
        }

        if (token.IsUsed || token.ExpiresAt < DateTime.UtcNow)
        {
            return false;
        }

        if (token.Otp != request.Otp.Trim())
        {
            return false;
        }

        return true;
    }

    public async Task ResetPasswordAsync(
        ResetPasswordRequestDto request)
    {
        if (request.NewPassword != request.ConfirmPassword)
        {
            throw new InvalidOperationException(
                "Passwords do not match."
            );
        }

        var student = await _studentRepository.GetByEmailAsync(
            request.Email.Trim().ToLower()
        );

        if (student is null || !student.IsActive)
        {
            throw new InvalidOperationException(
                "Invalid password reset request."
            );
        }

        var token = await _studentRepository
            .GetLatestPasswordResetTokenAsync(
                request.Email.Trim().ToLower()
            );

        if (token is null ||
            token.IsUsed ||
            token.ExpiresAt < DateTime.UtcNow ||
            token.Otp != request.Otp.Trim())
        {
            throw new InvalidOperationException(
                "Invalid or expired verification code."
            );
        }

        student.PasswordHash = _passwordHasher.HashPassword(
            student,
            request.NewPassword
        );

        token.IsUsed = true;

        await _studentRepository.UpdatePasswordResetAsync(
            student,
            token
        );
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
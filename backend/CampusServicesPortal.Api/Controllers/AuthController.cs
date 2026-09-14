using CampusServicesPortal.Api.DTOs.Auth;
using CampusServicesPortal.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusServicesPortal.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(
        RegisterRequestDto request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request);

            return StatusCode(
                StatusCodes.Status201Created,
                response
            );
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new
            {
                message = exception.Message
            });
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(
        LoginRequestDto request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);

            return Ok(response);
        }
        catch (UnauthorizedAccessException exception)
        {
            return Unauthorized(new
            {
                message = exception.Message
            });
        }
    }
    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
    ForgotPasswordRequestDto request)
    {
        await _authService.ForgotPasswordAsync(request);

        return Ok(new
        {
            message = "If the email is registered, a verification code has been sent."
        });
    }

   
    [AllowAnonymous]
    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp(
    VerifyOtpRequestDto request)
    {
        var result = await _authService.VerifyOtpAsync(request);

        if (!result)
        {
            return BadRequest(new
            {
                message = "Invalid or expired verification code."
            });
        }

        return Ok(new
        {
            message = "Verification successful."
        });
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
    ResetPasswordRequestDto request)
    {
        try
        {
            await _authService.ResetPasswordAsync(request);

            return Ok(new
            {
                message = "Password reset successfully."
            });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
    }



}
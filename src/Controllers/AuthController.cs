using BlogApp.Dto;
using BlogApp.Services;
using BlogApp.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers;

[ApiController]
[Route("/auth")]
public class AuthController(Mailer mailer, AuthService authService) : ControllerBase
{
    private readonly Mailer _mailer = mailer;

    private readonly AuthService _authService = authService;

    [AllowAnonymous]
    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] SignupRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(
                new
                {
                    success = false,
                    message = "Invalid Payload",
                    errors,
                }
            );
        }

        try
        {
            if (await _authService.EmailExists(request.Email))
            {
                return BadRequest(new { success = false, message = "Email already Registered" });
            }

            string hashedPassword = BcryptUtil.HashPassword(request.Password);

            var user = await _authService.RegisterUser(request.Email, hashedPassword);

            var profile = await _authService.CreateProfile(
                user.Id,
                request.FirstName,
                request.LastName
            );

            var otp = new Random().Next(100000, 999999).ToString();

            await _authService.SaveOtp(request.Email, otp);

            _ = Task.Run(async () => await _mailer.SendEmailAsync(request.Email, otp));

            return Ok(
                new
                {
                    success = true,
                    message = "Signup successful, please check your email for the OTP.",
                }
            );
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new
                {
                    success = false,
                    message = "Server error while Signup",
                    error = ex.Message,
                }
            );
        }
    }

    [AllowAnonymous]
    [HttpPost("verify-user")]
    public async Task<IActionResult> VerifyUser([FromBody] VerifyUserRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(
                new
                {
                    success = false,
                    message = "Invalid Payload",
                    errors,
                }
            );
        }

        try
        {
            var verificationResult = await _authService.VerifyUser(request.Email, request.Otp);

            if (!verificationResult.Success)
            {
                return BadRequest(new { success = false, message = verificationResult.Message });
            }

            return Ok(new { success = true, message = "User's Email Verified" });
        }
        catch (Exception ex)
        {
            return BadRequest(
                new
                {
                    success = false,
                    message = "Server Error While Verifying User",
                    error = ex.Message,
                }
            );
        }
    }

    [AllowAnonymous]
    [HttpPut("resend-otp")]
    public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequest body)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(
                new
                {
                    success = false,
                    message = "Invalid Payload",
                    errors,
                }
            );
        }

        try
        {
            string otp = new Random().Next(100000, 999999).ToString();

            var result = await _authService.UpdateOtp(body.Email, otp);

            if (!result)
            {
                return BadRequest(
                    new { success = false, message = "Email is Verified or not Registered" }
                );
            }

            _ = Task.Run(async () => await _mailer.SendEmailAsync(body.Email, otp));

            return Ok(new { success = true, message = "OTP sent again" });
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new
                {
                    success = false,
                    message = "Error while Resending-Otp",
                    error = ex.Message,
                }
            );
        }
    }
}

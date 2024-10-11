using BlogApp.Dto;
using BlogApp.Models;
using BlogApp.Services;
using BlogApp.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers;

[Controller]
[Route("/user")]
public class UserController(UserService userService, Uploader uploader) : ControllerBase
{
    private readonly UserService _userService = userService;

    private readonly Uploader _uploader = uploader;

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest body)
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
            var user = await _userService.GetUser(body.Email);

            if (user == null || user.Profile == null)
            {
                return BadRequest(new { success = false, message = "Email not Registered" });
            }
            else if (!user.Verified)
            {
                return Unauthorized(new { success = false, message = "Email not Verified" });
            }

            if (!BcryptUtil.VerifyPassword(body.Password, user.Password))
            {
                return Unauthorized(new { success = false, message = "Wrong Password" });
            }

            var token = JwtUtil.GenerateJwt(
                user.Id.ToString(),
                user.Email,
                user.Profile.Id.ToString(),
                user.Verified
            );

            Response.Cookies.Append(
                "Token",
                token,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(3),
                }
            );

            Response.Headers.Append("Authorization", $"Bearer {token}");

            return Ok(
                new
                {
                    success = true,
                    message = "Login Successfull",
                    id = user.Id,
                    profileId = user.Profile.Id,
                    token,
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
                    message = "Server error while login",
                    error = ex.Message,
                }
            );
        }
    }

    [Authorize]
    [HttpGet("get-profile")]
    public async Task<IActionResult> GetUserProfile()
    {
        var userId = User.FindFirst("Id")?.Value;

        if (userId == null)
        {
            return BadRequest(new { success = false, message = "Please Loging First" });
        }

        try
        {
            Profile? profile;

            if (Guid.TryParse(userId, out Guid userGuid))
            {
                profile = await _userService.GetProfile(userGuid);

                if (profile == null)
                {
                    return Unauthorized(
                        new { success = false, message = "User Profile not found" }
                    );
                }
            }
            else
            {
                return BadRequest(new { success = false, message = "Invalid Credentials" });
            }

            return Ok(
                new
                {
                    success = true,
                    message = "User's profile fetched successfully",
                    data = profile,
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
                    message = "Server error while getting User's profile",
                    error = ex.Message,
                }
            );
        }
    }

    [Authorize]
    [HttpPut("update-profile")]
    public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileRequest request)
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

        var userId = User.FindFirst("Id")?.Value;

        var verifiedStr = User.FindFirst("Verified")?.Value;
        bool verified = false;

        if (userId == null)
        {
            return BadRequest(new { success = false, message = "Please Loging First" });
        }

        if (!string.IsNullOrEmpty(verifiedStr))
        {
            _ = bool.TryParse(verifiedStr, out verified);
        }

        if (!verified)
        {
            return Unauthorized(new { success = false, message = "Email not Verified" });
        }

        try
        {
            Profile? profile;

            if (Guid.TryParse(userId, out Guid userGuid))
            {
                profile = await _userService.GetProfile(userGuid);

                if (profile == null)
                {
                    return Unauthorized(
                        new { success = false, message = "User Profile not found" }
                    );
                }
            }
            else
            {
                return BadRequest(new { success = false, message = "Invalid userId" });
            }

            profile.FirstName = request.FirstName ?? profile.FirstName;
            profile.LastName = request.LastName ?? profile.LastName;

            if (request.Image != null)
            {
                profile.Image = await _uploader.UploadImageAsync(request.Image);
            }

            await _userService.UpdateProfile(profile);

            return Ok(
                new
                {
                    success = true,
                    message = "Updated Profile Successfully",
                    image = profile.Image,
                    firstName = profile.FirstName,
                    lastName = profile.LastName,
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
                    message = "Server error while updating Profile",
                    error = ex.Message,
                }
            );
        }
    }
}

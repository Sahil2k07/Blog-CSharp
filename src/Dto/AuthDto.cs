using System.ComponentModel.DataAnnotations;

namespace BlogApp.Dto;

public class SignupRequest
{
    [EmailAddress(ErrorMessage = "Invalid Email")]
    [Required(ErrorMessage = "Email Required")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password Required")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "FirstName Required")]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "LastName Required")]
    public required string LastName { get; set; }
}

public class VerifyUserRequest
{
    [EmailAddress(ErrorMessage = "Invalid Email")]
    [Required(ErrorMessage = "Email Required")]
    public required string Email { get; set; }

    [StringLength(6, ErrorMessage = "OTP Length must be six")]
    [Required(ErrorMessage = "Verificaton OTP Required")]
    public required string Otp { get; set; }
}

public class ResendOtpRequest
{
    [EmailAddress(ErrorMessage = "Invalid Email")]
    [Required(ErrorMessage = "Email Required")]
    public required string Email { get; set; }
}

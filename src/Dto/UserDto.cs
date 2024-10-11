using System.ComponentModel.DataAnnotations;

namespace BlogApp.Dto;

public class LoginRequest
{
    [EmailAddress(ErrorMessage = "Invalid Email")]
    [Required(ErrorMessage = "Email Required")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password Required")]
    public required string Password { get; set; }
}

public class UpdateProfileRequest
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public IFormFile? Image { get; set; }
}

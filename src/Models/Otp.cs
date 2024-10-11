using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Models;

[Index(nameof(Email), IsUnique = true)]
public class Otp
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, EmailAddress]
    public required string Email { get; set; }

    [Required, StringLength(6)]
    public required string VerificationOtp { get; set; }

    public DateTime CreatedAT { get; set; } = DateTime.UtcNow;
}

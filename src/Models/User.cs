using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Models;

[Index(nameof(Email), IsUnique = true)]
public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [EmailAddress, Required]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }

    [Required, DefaultValue(false)]
    public bool Verified { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public required DateTime UpdatedAt { get; set; }

    public virtual Profile? Profile { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Models;

[Index(nameof(UserId), IsUnique = true)]
public class Profile
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [ForeignKey("User")]
    [Required]
    public required Guid UserId { get; set; }

    [Required]
    public required string FirstName { get; set; }

    [Required]
    public required string LastName { get; set; }

    [Url, Required]
    public required string Image { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public required DateTime UpdatedAt { get; set; }

    public virtual User? User { get; set; }
}

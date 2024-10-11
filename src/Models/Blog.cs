using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Models;

[Index(nameof(Id), nameof(ProfileId))]
public class Blog
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [ForeignKey("Profile")]
    [Required]
    public required Guid ProfileId { get; set; }

    [Required]
    public required string Title { get; set; }

    [Required]
    public required string Content { get; set; }

    [Required]
    public required string[] Tags { get; set; } = [];

    [Required]
    public required bool Published { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public required DateTime UpdatedAt { get; set; }
}

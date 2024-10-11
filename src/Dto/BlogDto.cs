using System.ComponentModel.DataAnnotations;

namespace BlogApp.Dto;

public class CreateBlogRequest
{
    [Required(ErrorMessage = "Blog Title Required")]
    public required string Title { get; set; }

    [Required(ErrorMessage = "Blog Content Required")]
    public required string Content { get; set; }

    public string[] Tags { get; set; } = [];
}

public class UpdateBlogRequest
{
    [Required(ErrorMessage = "Blog Title Required")]
    public required string Title { get; set; }

    [Required(ErrorMessage = "Blog Content Required")]
    public required string Content { get; set; }

    [Required(ErrorMessage = "Tags Required")]
    public required string[] Tags { get; set; } = [];
}

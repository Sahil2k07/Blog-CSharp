using BlogApp.Dto;
using BlogApp.Models;
using BlogApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers;

[Controller]
[Route("/blog")]
public class BlogController(BlogService blogService) : ControllerBase
{
    private readonly BlogService _blogService = blogService;

    [Authorize]
    [HttpPost("create-blog")]
    public async Task<IActionResult> CreateBlog([FromBody] CreateBlogRequest body)
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

        var profileId = User.FindFirst("ProfileId")?.Value;

        if (profileId == null)
        {
            return Unauthorized(new { success = false, message = "Please Login first" });
        }

        try
        {
            Blog blog;

            if (Guid.TryParse(profileId, out Guid profileGuid))
            {
                blog = await _blogService.CreateBlogAsync(
                    profileGuid,
                    body.Title,
                    body.Content,
                    body.Tags
                );
            }
            else
            {
                return BadRequest(new { success = false, message = "Invalid Credentials" });
            }

            return Ok(
                new
                {
                    success = true,
                    message = "Blog created Successfully",
                    data = blog,
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
                    message = "Server error while Creating Blog Post",
                    error = ex.Message,
                }
            );
        }
    }

    [Authorize]
    [HttpGet("user-blogs")]
    public async Task<IActionResult> GetUserBlogs()
    {
        var profileId = User.FindFirst("ProfileId")?.Value;

        if (profileId == null)
        {
            return Unauthorized(new { success = false, message = "Please Login first" });
        }

        try
        {
            List<Blog> blogs;

            if (Guid.TryParse(profileId, out Guid profileGuid))
            {
                blogs = await _blogService.GetUserBlogsAsync(profileGuid);
            }
            else
            {
                return BadRequest(new { success = false, message = "Invalid Credentials" });
            }

            return Ok(
                new
                {
                    success = true,
                    message = "User's Blog Posts fetched Successfully",
                    data = blogs,
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
                    message = "Server error while getting User's Blog Posts",
                    error = ex.Message,
                }
            );
        }
    }

    [AllowAnonymous]
    [HttpGet("get-blog/{id}")]
    public async Task<IActionResult> GetBlog(Guid id)
    {
        try
        {
            var blog = await _blogService.GetBlogAsync(id);

            if (blog == null)
            {
                return BadRequest(new { success = false, message = "Invalid Id" });
            }

            return Ok(
                new
                {
                    success = true,
                    message = "Fetched Blog post successfully",
                    data = blog,
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
                    message = "Server error while getting Blog Post",
                    error = ex.Message,
                }
            );
        }
    }

    [AllowAnonymous]
    [HttpGet("get-blogs")]
    public async Task<IActionResult> GetAllBlogs([FromQuery] int page = 1)
    {
        try
        {
            var blogs = await _blogService.GetAllBlogsAsync(page);

            if (blogs == null)
            {
                return NotFound(new { success = false, message = "No more Blog posts" });
            }

            return Ok(
                new
                {
                    success = true,
                    message = "Fetched All Blogs successfully",
                    data = blogs,
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
                    message = "Server Error while fetching blogs",
                    error = ex.Message,
                }
            );
        }
    }

    [Authorize]
    [HttpPut("update-blog/{id}")]
    public async Task<IActionResult> UpdateBlog(Guid id, [FromBody] UpdateBlogRequest body)
    {
        var profileId = User.FindFirst("ProfileId")?.Value;

        if (profileId == null)
        {
            return Unauthorized(new { success = false, message = "Please Login first" });
        }

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
            Blog? blog;

            if (Guid.TryParse(profileId, out Guid profileGuid))
            {
                blog = await _blogService.UpdateBlogAsync(
                    profileGuid,
                    id,
                    body.Title,
                    body.Content,
                    body.Tags
                );

                if (blog == null)
                {
                    return BadRequest(new { success = false, message = "User's blog not found" });
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
                    message = "Blog updated successfully",
                    data = blog,
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
                    message = "Server error while updating blog",
                    error = ex.Message,
                }
            );
        }
    }

    [Authorize]
    [HttpDelete("delete-blog/{id}")]
    public async Task<IActionResult> DeleteBlog(Guid id)
    {
        var profileId = User.FindFirst("ProfileId")?.Value;

        if (profileId == null)
        {
            return Unauthorized(new { success = false, message = "Please Login first" });
        }

        try
        {
            bool deletedBlog;

            if (Guid.TryParse(profileId, out Guid profileGuid))
            {
                deletedBlog = await _blogService.DeleteBlogAsync(id, profileGuid);
            }
            else
            {
                return Unauthorized(new { success = false, message = "Invalid Credentials" });
            }

            if (!deletedBlog)
            {
                return BadRequest(new { success = false, message = "Invalid id or User" });
            }

            return Ok(new { success = true, message = "Blog deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new
                {
                    success = false,
                    message = "Server error while Deleting Blog",
                    error = ex.Message,
                }
            );
        }
    }
}

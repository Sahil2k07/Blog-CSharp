using BlogApp.Database;
using BlogApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Services;

public class BlogService(BlogAppDbContext dbContext)
{
    private readonly BlogAppDbContext _dbContext = dbContext;

    public async Task<Blog> CreateBlogAsync(
        Guid profileId,
        string title,
        string content,
        string[] tags
    )
    {
        var blog = new Blog
        {
            ProfileId = profileId,
            Title = title,
            Content = content,
            Tags = tags,
            UpdatedAt = DateTime.UtcNow,
            Published = true,
        };

        _dbContext.Blog.Add(blog);
        await _dbContext.SaveChangesAsync();

        return blog;
    }

    public async Task<List<Blog>> GetUserBlogsAsync(Guid profileId)
    {
        var blogs = await _dbContext.Blog.Where(b => b.ProfileId == profileId).ToListAsync();

        return blogs;
    }

    public async Task<List<Blog>> GetAllBlogsAsync(int pageNumber)
    {
        if (pageNumber < 1)
        {
            pageNumber = 1;
        }

        int PageSize = 15;

        var blogs = await _dbContext
            .Blog.OrderByDescending(b => b.UpdatedAt)
            .Skip((pageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        return blogs;
    }

    public async Task<Blog?> GetBlogAsync(Guid id)
    {
        var blog = await _dbContext.Blog.SingleOrDefaultAsync(b => b.Id == id);

        return blog;
    }

    public async Task<Blog?> UpdateBlogAsync(
        Guid profileId,
        Guid id,
        string title,
        string content,
        string[] tags
    )
    {
        var blog = await _dbContext.Blog.SingleOrDefaultAsync(b =>
            b.ProfileId == profileId && b.Id == id
        );

        if (blog == null)
        {
            return null;
        }

        blog.Title = title;
        blog.Content = content;
        blog.Tags = tags;
        blog.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return blog;
    }

    public async Task<bool> DeleteBlogAsync(Guid id, Guid profileId)
    {
        var blog = await _dbContext.Blog.SingleOrDefaultAsync(b =>
            b.Id == id && b.ProfileId == profileId
        );

        if (blog == null)
            return false;

        _dbContext.Blog.Remove(blog);
        await _dbContext.SaveChangesAsync();

        return true;
    }
}

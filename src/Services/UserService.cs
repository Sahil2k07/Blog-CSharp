using BlogApp.Database;
using BlogApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Services;

public class UserService(BlogAppDbContext dbContext)
{
    private readonly BlogAppDbContext _dbContext = dbContext;

    public async Task<User?> GetUser(string email)
    {
        var user = await _dbContext
            .User.Include(u => u.Profile)
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Email == email);

        return user;
    }

    public async Task<Profile?> GetProfile(Guid userId)
    {
        var profile = await _dbContext
            .Profile.AsNoTracking()
            .SingleOrDefaultAsync(p => p.UserId == userId);

        return profile;
    }

    public async Task UpdateProfile(Profile profile)
    {
        profile.UpdatedAt = DateTime.UtcNow;

        _dbContext.Profile.Update(profile);

        await _dbContext.SaveChangesAsync();
    }
}

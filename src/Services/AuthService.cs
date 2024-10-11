using BlogApp.Database;
using BlogApp.Models;
using BlogApp.Utils;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Services;

public class AuthService(BlogAppDbContext dbContext, BloomFilterUtil bloomFilterUtil)
{
    private readonly BlogAppDbContext _dbContext = dbContext;

    private readonly BloomFilterUtil _bloomFilterUtil = bloomFilterUtil;

    public async Task<bool> EmailExists(string email)
    {
        if (_bloomFilterUtil.CheckEmail(email))
        {
            return true;
        }

        return await _dbContext.User.AnyAsync(u => u.Email == email);
    }

    public async Task<User> RegisterUser(string email, string hashedPassword)
    {
        var user = new User
        {
            Email = email,
            Password = hashedPassword,
            UpdatedAt = DateTime.UtcNow,
        };

        _dbContext.User.Add(user);
        await _dbContext.SaveChangesAsync();

        _bloomFilterUtil.AddEmail(email);

        return user;
    }

    public async Task<Profile> CreateProfile(Guid userId, string firstName, string lastName)
    {
        var profile = new Profile
        {
            FirstName = firstName,
            LastName = lastName,
            Image = $"https://api.dicebear.com/5.x/initials/svg?seed={firstName} {lastName}",
            UserId = userId,
            UpdatedAt = DateTime.UtcNow,
        };

        _dbContext.Profile.Add(profile);
        await _dbContext.SaveChangesAsync();

        return profile;
    }

    public async Task SaveOtp(string email, string otp)
    {
        var newOtp = new Otp { Email = email, VerificationOtp = otp };

        _dbContext.Otp.Add(newOtp);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> UpdateOtp(string email, string otp)
    {
        var storedOtp = await _dbContext.Otp.SingleOrDefaultAsync(o => o.Email == email);

        if (storedOtp == null)
            return false;

        storedOtp.VerificationOtp = otp;

        await _dbContext.SaveChangesAsync();

        return true;
    }

    public class VerifyUserResult
    {
        public bool Success { get; set; }

        public required string Message { get; set; }
    }

    public async Task<VerifyUserResult> VerifyUser(string email, string otp)
    {
        var user = await _dbContext.User.AsNoTracking().SingleOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            return new VerifyUserResult { Success = false, Message = "User not found." };
        }

        if (user.Verified)
        {
            return new VerifyUserResult { Success = false, Message = "User is already verified." };
        }

        var storedOtp = await _dbContext
            .Otp.AsNoTracking()
            .SingleOrDefaultAsync(o => o.Email == email);

        if (storedOtp == null || storedOtp.VerificationOtp != otp)
        {
            return new VerifyUserResult { Success = false, Message = "Invalid OTP." };
        }

        user.Verified = true;
        user.UpdatedAt = DateTime.UtcNow;

        _dbContext.User.Update(user);
        _dbContext.Otp.Remove(storedOtp);

        await _dbContext.SaveChangesAsync();

        return new VerifyUserResult { Success = true, Message = "User verified successfully." };
    }
}

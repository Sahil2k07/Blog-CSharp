using System.Text;
using BlogApp.Database;
using BlogApp.Middlewares;
using BlogApp.Services;
using BlogApp.Utils;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls(Env.GetString("APP_URL"));

// CORS setup
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        string allowedOrigin = Env.GetString("ALLOWED_ORIGIN")!;
        policy.WithOrigins(allowedOrigin).AllowAnyMethod().AllowAnyHeader();
    });
});

// Configure JWT authentication
var secretKey = Env.GetString("JWT_SECRET")!;
builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero,
        };
    });

// Add necessary services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddDbContext<BlogAppDbContext>();
builder.Services.AddScoped<Mailer>();
builder.Services.AddScoped<Uploader>();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<BlogService>();

// Add Bloom filter as a singleton
var bloomFilter = new BloomFilterUtil(size: 100000, numberOfHashFunctions: 3);
builder.Services.AddSingleton(bloomFilter);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();

app.UseMiddleware<AuthMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Ok(new { success = true, message = "Server is Active" }))
    .AllowAnonymous();

app.Run();

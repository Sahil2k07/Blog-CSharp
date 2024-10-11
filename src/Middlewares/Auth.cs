using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace BlogApp.Middlewares;

public class AuthMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint != null)
        {
            var allowAnonymousMetadata = endpoint.Metadata.GetMetadata<AllowAnonymousFilter>();
            if (allowAnonymousMetadata != null)
            {
                await _next(context);
                return;
            }
        }

        // Token retrieval logic
        string? token = null;

        if (context.Request.Cookies.ContainsKey("Token"))
        {
            token = context.Request.Cookies["Token"];
        }
        else if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            if (authHeader.ToString().StartsWith("Bearer "))
            {
                token = authHeader.ToString()["Bearer ".Length..].Trim();
            }
            else
            {
                token = authHeader.ToString();
            }
        }

        // Token validation logic
        if (token != null)
        {
            var claims = JwtUtil.VerifyJwt(token);
            if (claims != null)
            {
                var claimsIdentity = new ClaimsIdentity(
                    claims.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!)),
                    "Custom"
                );

                // Set the HttpContext.User to a new ClaimsPrincipal
                context.User = new ClaimsPrincipal(claimsIdentity);
            }
            else
            {
                await UnauthorizedResponse(context, "Unauthorized access. Invalid token.");
                return;
            }
        }

        await _next(context);
    }

    private static async Task UnauthorizedResponse(HttpContext context, string message)
    {
        context.Response.StatusCode = 401;
        context.Response.ContentType = "application/json";

        var response = new { success = false, message };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

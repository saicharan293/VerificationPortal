
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

public class SessionValidationMiddleware
{
    private readonly RequestDelegate _next;
    public SessionValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant();

        // ALLOW LOGIN AND OTHER PUBLIC PAGES
        if(path != null && (path.Contains("/account/login") || path.Contains("/account/logout")))
        {
            await _next(context);
            return;
        }

        if(path != null && path.StartsWith("/verificationdashboard")){
            var userId = context.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    context.Session.SetString("UserId", userId);
                }
            }
        }
        await _next(context);
    }
}
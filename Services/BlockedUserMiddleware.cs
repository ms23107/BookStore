using BookStore.Models;
using Microsoft.AspNetCore.Identity;

namespace BookStore.Services
{
    public class BlockedUserMiddleware
{
    private readonly RequestDelegate _next;

    public BlockedUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, UserManager<DefaultUser> userManager)
    {
        if (!context.Request.Path.StartsWithSegments("/Account/Blocked") &&
            !context.Request.Path.StartsWithSegments("/Account/Logout"))
            {
            var user = await userManager.GetUserAsync(context.User);
            if (user != null && user.IsBlocked)
            {
                context.Response.Redirect("/Account/Blocked");
                return;
            }
        }

        await _next(context);
    }
}
}

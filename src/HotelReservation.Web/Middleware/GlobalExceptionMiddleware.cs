using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelReservation.Web.Middleware;

/// <summary>
/// Catches unhandled exceptions from the request pipeline and redirects to a
/// friendly error page, while logging the failure. Mirrors the behaviour of
/// <c>app.UseExceptionHandler</c> but routes to the Razor Error page with a message.
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for {Method} {Path}",
                context.Request.Method, context.Request.Path);

            // For API requests, return JSON instead of redirecting.
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(
                    System.Text.Json.JsonSerializer.Serialize(new { message = "An internal server error occurred." }));
                return;
            }

            // For browser requests, redirect to the friendly Error page.
            if (!context.Response.HasStarted)
            {
                context.Response.Redirect("/Error?message=" + Uri.EscapeDataString(
                    "We could not process your request. Please try again or contact support."));
            }
        }
    }
}

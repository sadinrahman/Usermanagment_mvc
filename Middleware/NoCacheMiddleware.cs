using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class NoCacheMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<NoCacheMiddleware> _logger;

    public NoCacheMiddleware(RequestDelegate next, ILogger<NoCacheMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("ghdvsghdfvdjvdbhdfdfjhfduhf Processing request: {Method} {Path}", context.Request.Method, context.Request.Path);
        context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, proxy-revalidate";
        context.Response.Headers["Pragma"] = "no-cache";
        context.Response.Headers["Expires"] = "0";

        await _next(context);
    }
}

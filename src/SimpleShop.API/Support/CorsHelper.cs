namespace SimpleShop.API.Support;

public static class CorsHelper
{
    public static string[] GetAllowedOrigins(IConfiguration configuration)
    {
        var env = Environment.GetEnvironmentVariable("CORS_ORIGINS");
        if (!string.IsNullOrWhiteSpace(env))
            return env.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var fromConfig = configuration.GetSection("AllowedOrigins").Get<string[]>();
        return fromConfig is { Length: > 0 } ? fromConfig : new[] { "http://localhost:3000" };
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SimpleShop.API.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SimpleShop.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    /// <summary>Login with admin credentials to receive a JWT token.</summary>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var adminEmail = _config["AdminAccount:Email"];
        var adminPassword = _config["AdminAccount:Password"];
        var adminRole = _config["AdminAccount:Role"] ?? "Admin";

        if (dto.Email != adminEmail || dto.Password != adminPassword)
            return Unauthorized(new { message = "Invalid email or password." });

        var jwtSettings = _config.GetSection("JwtSettings");
        var secret = jwtSettings["Secret"]!;
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expirationHours = int.Parse(jwtSettings["ExpirationHours"] ?? "8");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddHours(expirationHours);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, dto.Email),
            new Claim(ClaimTypes.Email, dto.Email),
            new Claim(ClaimTypes.Role, adminRole)
        };

        var token = new JwtSecurityToken(issuer, audience, claims, expires: expires, signingCredentials: creds);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new LoginResponseDto
        {
            Token = tokenString,
            Email = dto.Email,
            Role = adminRole,
            ExpiresAt = expires
        });
    }
}

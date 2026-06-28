using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SimpleShop.Repo.Models;
using SimpleShop.Repo.Repositories;
using SimpleShop.Service.Dtos;

namespace SimpleShop.Service.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly ICartRepository _carts;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository users, ICartRepository carts, IConfiguration configuration)
    {
        _users = users;
        _carts = carts;
        _configuration = configuration;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLower();
        if (await _users.EmailExistsAsync(email))
            throw new InvalidOperationException("Email already exists.");

        var user = new AppUser
        {
            Name = request.Name.Trim(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "User"
        };

        await _users.AddAsync(user);
        await _users.SaveChangesAsync();
        await _carts.GetOrCreateByUserIdAsync(user.Id);

        return new AuthResponse(CreateToken(user), ToUserDto(user));
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _users.GetByEmailAsync(request.Email.Trim().ToLower());
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        await _carts.GetOrCreateByUserIdAsync(user.Id);
        return new AuthResponse(CreateToken(user), ToUserDto(user));
    }

    public async Task<UserDto> GetMeAsync(int userId)
    {
        var user = await _users.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found.");
        return ToUserDto(user);
    }

    private string CreateToken(AppUser user)
    {
        var secret = Environment.GetEnvironmentVariable("JWT_SECRET")
            ?? _configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("JWT_SECRET is not configured.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static UserDto ToUserDto(AppUser user) => new(user.Id, user.Name, user.Email, user.Role);
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Blog.Application.Auth;
using Blog.Application.Auth.DTOs;
using Blog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;


namespace Blog.Infrastructure.Services;

public class AuthService(Blog.Infrastructure.Persistence.BlogDbContext context, IConfiguration config) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var exists = await context.Users.AnyAsync(u => u.Email == request.Email);
        if (exists) throw new Exception("Email already registered");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "user"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return GenerateToken(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new Exception("Invalid credentials");

        return GenerateToken(user);
    }

    public async Task<AuthResponse> GetUserInfoAsync(string userId)
    {
        var user = await context.Users.FindAsync(Guid.Parse(userId));
        if (user == null) throw new Exception("User not found");

        return new AuthResponse
        {
            Username = user.Username,
            Role = user.Role,
            Token = "" // vacío, porque solo es para consulta
        };
    }

    private AuthResponse GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Username = user.Username,
            Role = user.Role
        };
    }
}
using Blog.Application.Auth.DTOs;

namespace Blog.Application.Auth;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> GetUserInfoAsync(string userId);
}
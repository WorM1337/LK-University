using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Personal_Cabinet_Uni.Data.Repositories;
using Personal_Cabinet_Uni.Models.DTO.Request;
using Personal_Cabinet_Uni.Models.DTO.Response;
using Personal_Cabinet_Uni.Models.Entities;
using Personal_Cabinet_Uni.Shared.Exceptions;
using Personal_Cabinet_Uni.Shared.Models.Enums;

namespace Personal_Cabinet_Uni.Services;

public class AuthService : IAuthService
{
    private readonly IProfileRepository _profileRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IProfileRepository profileRepository, IConfiguration configuration)
    {
        _profileRepository = profileRepository;
        _configuration = configuration;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        // Проверка существования пользователя с таким email
        if (await _profileRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            throw new ConflictException("Пользователь с таким email уже существует");
        }

        // Создание профиля
        var profile = new Profile
        {
            Name = request.Name,
            Surname = request.Surname,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Birthday = request.Birthday,
            Gender = request.Gender,
            Nationality = request.Nationality,
            PasswordHash = HashPassword(request.Password),
            Role = Role.User, // По умолчанию роль User
            CreatedAt = DateTime.UtcNow
        };

        await _profileRepository.CreateAsync(profile, cancellationToken);

        // Генерация токенов
        var (accessToken, refreshToken) = GenerateTokens(profile);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        // Поиск пользователя по email
        var profile = await _profileRepository.GetByEmailAsync(request.Email, cancellationToken);
        
        if (profile == null)
        {
            throw new UnauthorizedException("Неверный email или пароль");
        }

        // Проверка пароля
        if (!VerifyPassword(request.Password, profile.PasswordHash))
        {
            throw new UnauthorizedException("Неверный email или пароль");
        }

        // Генерация токенов
        var (accessToken, refreshToken) = GenerateTokens(profile);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        
        // Поиск пользователя по ID
        var profile = await _profileRepository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);
        if (profile == null)
        {
            throw new UnauthorizedException("Пользователь с таким refresh токеном не найден");
        }

        // Проверка срока действия refresh токена
        if (profile.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new UnauthorizedException("Refresh токен истек");
        }

        // Генерация новых токенов
        var (accessToken, refreshToken) = GenerateTokens(profile);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    private (string accessToken, string refreshToken) GenerateTokens(Profile profile)
    {
        var accessToken = GenerateJwtToken(profile);
        var refreshToken = GenerateRefreshToken();
        
        // Сохранение refresh токена в БД
        profile.RefreshToken = refreshToken;
        profile.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
            double.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "7"));
        
        _profileRepository.UpdateAsync(profile, CancellationToken.None).Wait();

        return (accessToken, refreshToken);
    }

    private string GenerateJwtToken(Profile profile)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? throw new BadRequestException("JWT SecretKey not configured")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, profile.Id.ToString()),
            new Claim(ClaimTypes.Email, profile.Email),
            new Claim(ClaimTypes.Name, profile.Name),
            new Claim(ClaimTypes.Surname, profile.Surname),
            new Claim(ClaimTypes.Role, profile.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["AccessTokenExpirationMinutes"] ?? "15")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal? GetClaimsFromJwtToken(string token)
    {
        try
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? throw new BadRequestException("JWT SecretKey not configured")));
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false, // Не проверяем время жизни при обновлении токена
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = key
            };

            return tokenHandler.ValidateToken(token, validationParameters, out _);
        }
        catch
        {
            return null;
        }
    }

    private string HashPassword(string password)
    {
        var salt = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32);

        var result = new byte[48];
        Array.Copy(salt, 0, result, 0, 16);
        Array.Copy(hash, 0, result, 16, 32);

        return Convert.ToBase64String(result);
    }

    private bool VerifyPassword(string password, string hash)
    {
        var bytes = Convert.FromBase64String(hash);
        var salt = new byte[16];
        Array.Copy(bytes, 0, salt, 0, 16);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
        var computedHash = pbkdf2.GetBytes(32);

        for (int i = 0; i < 32; i++)
        {
            if (bytes[i + 16] != computedHash[i])
            {
                return false;
            }
        }

        return true;
    }
}

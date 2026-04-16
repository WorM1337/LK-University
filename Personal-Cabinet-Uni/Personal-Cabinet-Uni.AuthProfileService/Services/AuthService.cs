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
        if (await _profileRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            throw new ConflictException("Пользователь с таким email уже существует");
        }

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
            Password = request.Password,
            Role = Role.Applicant,
            CreatedAt = DateTime.UtcNow
        };

        await _profileRepository.CreateAsync(profile, cancellationToken);

        var (accessToken, refreshToken) = GenerateTokens(profile);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var profile = await _profileRepository.GetByEmailAsync(request.Email, cancellationToken);
        
        if (profile == null)
        {
            throw new UnauthorizedException("Неверный email или пароль");
        }

        if (request.Password != profile.Password)
        {
            throw new UnauthorizedException("Неверный email или пароль");
        }

        var (accessToken, refreshToken) = GenerateTokens(profile);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        
        var profile = await _profileRepository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);
        if (profile == null)
        {
            throw new UnauthorizedException("Пользователь с таким refresh токеном не найден");
        }

        if (profile.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new UnauthorizedException("Refresh токен истек");
        }

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

    public async Task<ProfileResponse> CreateManagerAsync(CreateManagerRequest request, CancellationToken cancellationToken = default)
    {
        if (await _profileRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            throw new ConflictException("Пользователь с таким email уже существует");
        }

        if (request.Role != Role.Manager && request.Role != Role.MainManager)
        {
            throw new BadRequestException("Роль должна быть Manager или MainManager");
        }

        var profile = new Profile
        {
            Name = request.Name,
            Surname = request.Surname,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Birthday = request.Birthday.HasValue ? DateTime.SpecifyKind(request.Birthday.Value, DateTimeKind.Utc) : null,
            Gender = request.Gender ?? null,
            Nationality = request.Nationality,
            Password = request.Password,
            Role = request.Role,
            CreatedAt = DateTime.UtcNow
        };

        await _profileRepository.CreateAsync(profile, cancellationToken);

        return MapToProfileResponse(profile);
    }

    public async Task<ProfileResponse> EditManagerAsync(string email, EditManagerRequest request, CancellationToken cancellationToken = default)
    {
        var profile = await _profileRepository.GetByEmailAsync(email, cancellationToken);
        
        if (profile == null)
        {
            throw new NotFoundException("Менеджер не найден");
        }

        if (!string.IsNullOrEmpty(request.Name))
            profile.Name = request.Name;
        if (!string.IsNullOrEmpty(request.Surname))
            profile.Surname = request.Surname;
        if (!string.IsNullOrEmpty(request.LastName))
            profile.LastName = request.LastName;
        if (!string.IsNullOrEmpty(request.Phone))
            profile.Phone = request.Phone;
        if (request.Birthday.HasValue)
            profile.Birthday = DateTime.SpecifyKind(request.Birthday.Value, DateTimeKind.Utc);
        if (request.Gender.HasValue)
            profile.Gender = request.Gender.Value;
        if (request.Nationality != null)
            profile.Nationality = request.Nationality;
        if (request.Role.HasValue)
        {
            if (request.Role != Role.Manager && request.Role != Role.MainManager)
            {
                throw new BadRequestException("Роль должна быть Manager или MainManager");
            }
            profile.Role = request.Role.Value;
        }

        await _profileRepository.UpdateAsync(profile, cancellationToken);

        return MapToProfileResponse(profile);
    }

    public async Task<bool> DeleteManagerAsync(string email, CancellationToken cancellationToken = default)
    {
        var profile = await _profileRepository.GetByEmailAsync(email, cancellationToken);
        
        if (profile == null)
        {
            throw new NotFoundException("Менеджер не найден");
        }

        if (profile.Role == Role.Admin)
        {
            throw new ForbiddenException("Нельзя удалить администратора");
        }

        await _profileRepository.DeleteAsync(profile.Id, cancellationToken);

        return true;
    }

    public async Task<IEnumerable<ProfileResponse>> GetAllManagersAsync(CancellationToken cancellationToken = default)
    {
        var profiles = await _profileRepository.GetAllAsync(cancellationToken);
        
        var managers = profiles.Where(p => p.Role == Role.Manager || p.Role == Role.MainManager);
        
        return managers.Select(MapToProfileResponse);
    }

    public async Task<ProfileResponse?> GetManagerByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var profile = await _profileRepository.GetByEmailAsync(email, cancellationToken);
        
        if (profile == null)
        {
            return null;
        }

        return MapToProfileResponse(profile);
    }

    private ProfileResponse MapToProfileResponse(Profile profile)
    {
        return new ProfileResponse
        {
            Id = profile.Id,
            Name = profile.Name,
            Surname = profile.Surname,
            LastName = profile.LastName,
            Email = profile.Email,
            Phone = profile.Phone,
            Birthday = profile.Birthday,
            Gender = profile.Gender,
            Nationality = profile.Nationality,
            Role = profile.Role,
            CreatedAt = profile.CreatedAt
        };
    }
}

using Personal_Cabinet_Uni.Data.Repositories;
using Personal_Cabinet_Uni.Models.DTO.Request;
using Personal_Cabinet_Uni.Models.DTO.Response;
using Personal_Cabinet_Uni.Models.Entities;
using Personal_Cabinet_Uni.Shared.Exceptions;
using Personal_Cabinet_Uni.Shared.Models.Enums;

namespace Personal_Cabinet_Uni.Services;

public class ProfileService : IProfileService
{
    private readonly IProfileRepository _profileRepository;

    public ProfileService(IProfileRepository profileRepository)
    {
        _profileRepository = profileRepository;
    }

    public async Task<IEnumerable<ProfileResponse>> GetAllAsync(int page, int limit, Role? role = null, CancellationToken cancellationToken = default)
    {
        var profiles = await _profileRepository.GetAllAsync(page, limit, role, cancellationToken);
        return profiles.Select(MapToResponse);
    }

    public async Task<ProfileResponse?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var profile = await _profileRepository.GetByEmailAsync(email, cancellationToken);
        return profile != null ? MapToResponse(profile) : null;
    }

    public async Task<ProfileResponse> UpdateAsync(string email, EditProfileRequest request, CancellationToken cancellationToken = default)
    {
        var profile = await _profileRepository.GetByEmailAsync(email, cancellationToken)
            ?? throw new NotFoundException("Профиль не найден");

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
        
        if (!string.IsNullOrEmpty(request.Nationality))
            profile.Nationality = request.Nationality;

        await _profileRepository.UpdateAsync(profile, cancellationToken);

        return MapToResponse(profile);
    }

    public async Task ResetPasswordAsync(string email, CancellationToken cancellationToken = default)
    {
        var profile = await _profileRepository.GetByEmailAsync(email, cancellationToken)
            ?? throw new NotFoundException("Профиль не найден");

        var tempPassword = GenerateTempPassword();
        profile.Password = tempPassword;
        
        await _profileRepository.UpdateAsync(profile, cancellationToken);

    }

    public async Task<ProfileResponse> CreateManagerAsync(CreateManagerRequest request, CancellationToken cancellationToken = default)
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
            Birthday = request.Birthday.HasValue ? DateTime.SpecifyKind(request.Birthday.Value, DateTimeKind.Utc) : DateTime.MinValue,
            Gender = request.Gender ?? Gender.Male,
            Nationality = request.Nationality,
            Password = request.Password,
            Role = request.Role,
            CreatedAt = DateTime.UtcNow
        };

        await _profileRepository.CreateAsync(profile, cancellationToken);

        return MapToResponse(profile);
    }

    public async Task UpdateManagerAsync(string email, EditManagerRequest request, CancellationToken cancellationToken = default)
    {
        var profile = await _profileRepository.GetByEmailAsync(email, cancellationToken)
            ?? throw new NotFoundException("Профиль не найден");

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
        
        if (!string.IsNullOrEmpty(request.Nationality))
            profile.Nationality = request.Nationality;
        
        if (request.Role.HasValue)
            profile.Role = request.Role.Value;

        await _profileRepository.UpdateAsync(profile, cancellationToken);
    }

    private static ProfileResponse MapToResponse(Profile profile)
    {
        return new ProfileResponse
        {
            Name = profile.Name,
            Surname = profile.Surname,
            LastName = profile.LastName,
            Email = profile.Email,
            Phone = profile.Phone,
            Birthday = profile.Birthday,
            Gender = profile.Gender,
            Nationality = profile.Nationality
        };
    }

    private string GenerateTempPassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
        var random = new Random();
        var password = new char[12];
        
        for (int i = 0; i < password.Length; i++)
        {
            password[i] = chars[random.Next(chars.Length)];
        }
        
        return new string(password);
    }
}

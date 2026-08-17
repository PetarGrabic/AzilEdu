using System.Net.Http.Headers;
using AzilEdu.Shared.DTOs;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace AzilEdu.App.Services;

public class CurrentUserService
{
    private const string StorageKey = "aziledu.auth.session";

    private readonly HttpClient _httpClient;
    private readonly ProtectedLocalStorage _storage;

    public CurrentUserService(
        HttpClient httpClient,
        ProtectedLocalStorage storage)
    {
        _httpClient = httpClient;
        _storage = storage;
    }

    public LoggedUserDto? User { get; private set; }
    public string? AccessToken { get; private set; }
    public DateTime? ExpiresAtUtc { get; private set; }
    public bool IsInitialized { get; private set; }
    public bool IsLoggedIn => User is not null && !IsExpired;

    private bool IsExpired =>
        !ExpiresAtUtc.HasValue || ExpiresAtUtc.Value <= DateTime.UtcNow;

    public event Action? UserChanged;

    public async Task InitializeAsync()
    {
        if (IsInitialized)
            return;

        try
        {
            var stored = await _storage.GetAsync<LoginResponseDto>(StorageKey);

            if (stored.Success &&
                stored.Value is not null &&
                stored.Value.ExpiresAtUtc > DateTime.UtcNow.AddSeconds(30) &&
                !string.IsNullOrWhiteSpace(stored.Value.AccessToken))
            {
                ApplySession(stored.Value);
            }
            else
            {
                await ClearSessionAsync();
            }
        }
        catch
        {
            await ClearSessionAsync();
        }
        finally
        {
            IsInitialized = true;
            UserChanged?.Invoke();
        }
    }

    public async Task LoginAsync(LoginResponseDto response)
    {
        ApplySession(response);
        await _storage.SetAsync(StorageKey, response);
        UserChanged?.Invoke();
    }

    public async Task LogoutAsync()
    {
        await ClearSessionAsync();
        UserChanged?.Invoke();
    }

    public bool HasRole(string role)
    {
        return User?.Roles.Contains(role) == true;
    }

    public bool HasAnyRole(params string[] roles)
    {
        return User?.Roles.Any(roles.Contains) == true;
    }

    private void ApplySession(LoginResponseDto response)
    {
        User = response.User;
        AccessToken = response.AccessToken;
        ExpiresAtUtc = response.ExpiresAtUtc;
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", response.AccessToken);
    }

    private async Task ClearSessionAsync()
    {
        User = null;
        AccessToken = null;
        ExpiresAtUtc = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;
        await _storage.DeleteAsync(StorageKey);
    }
}

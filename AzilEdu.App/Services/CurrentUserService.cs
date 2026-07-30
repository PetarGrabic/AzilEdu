using AzilEdu.Shared.DTOs;

namespace AzilEdu.App.Services;

public class CurrentUserService
{
    public LoggedUserDto? User { get; private set; }

    public bool IsLoggedIn => User is not null;

    public event Action? UserChanged;

    public void Login(LoggedUserDto user)
    {
        User = user;
        UserChanged?.Invoke();
    }

    public void Logout()
    {
        User = null;
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
}

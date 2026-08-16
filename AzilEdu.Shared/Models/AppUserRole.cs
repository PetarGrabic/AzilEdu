namespace AzilEdu.Shared.Models;

public class AppUserRole
{
    public int AppUserId { get; set; }
    public AppUser? AppUser { get; set; }

    public int AppRoleId { get; set; }
    public AppRole? AppRole { get; set; }
}
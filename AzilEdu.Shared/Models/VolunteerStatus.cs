namespace AzilEdu.Shared.Models;

public class VolunteerStatus
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Volunteer> Volunteers { get; set; } = new List<Volunteer>();
}

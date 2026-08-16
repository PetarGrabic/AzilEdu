namespace AzilEdu.Shared.Models;

public class VolunteerTaskStatus
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<VolunteerTask> Tasks { get; set; } = new List<VolunteerTask>();
}
namespace AzilEdu.Shared.Models;

public class VolunteerTask
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Notes { get; set; } = string.Empty;

    public int? VolunteerId { get; set; }
    public Volunteer? Volunteer { get; set; }

    public int? AnimalId { get; set; }
    public Animal? Animal { get; set; }

    public int VolunteerTaskStatusId { get; set; }
    public VolunteerTaskStatus? VolunteerTaskStatus { get; set; }

    public int VolunteerTaskTypeId { get; set; }
    public VolunteerTaskType? VolunteerTaskType { get; set; }
}
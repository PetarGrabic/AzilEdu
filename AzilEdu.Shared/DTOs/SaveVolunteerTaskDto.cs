namespace AzilEdu.Shared.DTOs;

public class SaveVolunteerTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Notes { get; set; } = string.Empty;

    public int? VolunteerId { get; set; }
    public int? AnimalId { get; set; }
    public int VolunteerTaskStatusId { get; set; }
    public int VolunteerTaskTypeId { get; set; }
}
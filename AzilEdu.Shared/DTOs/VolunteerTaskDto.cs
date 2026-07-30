namespace AzilEdu.Shared.DTOs;

public class VolunteerTaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Notes { get; set; } = string.Empty;

    public int? VolunteerId { get; set; }
    public string VolunteerName { get; set; } = string.Empty;

    public int? AnimalId { get; set; }
    public string AnimalName { get; set; } = string.Empty;

    public int VolunteerTaskStatusId { get; set; }
    public string Status { get; set; } = string.Empty;

    public int VolunteerTaskTypeId { get; set; }
    public string Type { get; set; } = string.Empty;
}
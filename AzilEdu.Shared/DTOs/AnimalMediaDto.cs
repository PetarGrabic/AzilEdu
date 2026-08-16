namespace AzilEdu.Shared.DTOs;

public class AnimalMediaDto
{
    public int Id { get; set; }
    public int AnimalId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string MediaType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? Caption { get; set; }
    public bool IsCover { get; set; }
    public int SortOrder { get; set; }
    public DateTime UploadedAt { get; set; }
}
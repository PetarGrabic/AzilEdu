namespace AzilEdu.Shared.Models;

public enum AnimalMediaType
{
    Image = 1,
    Video = 2
}

public class AnimalMedia
{
    public int Id { get; set; }

    public int AnimalId { get; set; }
    public Animal? Animal { get; set; }

    public string StoredFileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public AnimalMediaType MediaType { get; set; }
    public long FileSize { get; set; }
    public string? Caption { get; set; }
    public bool IsCover { get; set; }
    public int SortOrder { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
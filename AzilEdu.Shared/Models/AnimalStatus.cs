namespace AzilEdu.Shared.Models;

public class AnimalStatus
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Animal> Animals { get; set; } = new List<Animal>();
}
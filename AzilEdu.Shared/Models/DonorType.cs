namespace AzilEdu.Shared.Models;

public class DonorType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Donor> Donors { get; set; } = new List<Donor>();
}

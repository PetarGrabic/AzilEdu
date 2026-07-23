namespace AzilEdu.Shared.DTOs;

public class DashboardSummaryDto
{
    public int AnimalsCount { get; set; }
    public int AvailableAnimalsCount { get; set; }
    public int ActiveVolunteersCount { get; set; }
    public int OpenVolunteerTasksCount { get; set; }
    public int ActiveDonorsCount { get; set; }
    public int EmployeesCount { get; set; }

    public int DonationsCount { get; set; }
    public int PendingDonationsCount { get; set; }
    public decimal MoneyDonationsTotal { get; set; }
    public decimal EstimatedMaterialDonationsTotal { get; set; }
    public int OverdueVolunteerTasksCount { get; set; }
}

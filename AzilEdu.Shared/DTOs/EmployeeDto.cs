namespace AzilEdu.Shared.DTOs;

public class EmployeeDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string EmployeeNumber { get; set; } = string.Empty;
    public DateTime? HireDate { get; set; }
    public string Notes { get; set; } = string.Empty;
    public int EmployeePositionId { get; set; }
    public string Position { get; set; } = string.Empty;
    public int EmployeeStatusId { get; set; }
    public string Status { get; set; } = string.Empty;
}

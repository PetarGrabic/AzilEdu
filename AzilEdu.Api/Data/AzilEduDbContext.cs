using AzilEdu.Shared.Models;
using Microsoft.EntityFrameworkCore;
 
namespace AzilEdu.Api.Data;
 
public class AzilEduDbContext : DbContext
{
    public AzilEduDbContext(DbContextOptions<AzilEduDbContext> options)
        : base(options)
    {
    }
 
    public DbSet<Animal> Animals => Set<Animal>();
    public DbSet<HousingUnit> HousingUnits => Set<HousingUnit>();
}
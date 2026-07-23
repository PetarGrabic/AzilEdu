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
    public DbSet<AnimalStatus> AnimalStatuses => Set<AnimalStatus>();

    public DbSet<Volunteer> Volunteers => Set<Volunteer>();
    public DbSet<VolunteerStatus> VolunteerStatuses => Set<VolunteerStatus>();

    public DbSet<Donor> Donors => Set<Donor>();
    public DbSet<DonorType> DonorTypes => Set<DonorType>();
    public DbSet<DonorStatus> DonorStatuses => Set<DonorStatus>();

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<EmployeePosition> EmployeePositions => Set<EmployeePosition>();
    public DbSet<EmployeeStatus> EmployeeStatuses => Set<EmployeeStatus>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Animal>()
            .HasOne(animal => animal.AnimalStatus)
            .WithMany(status => status.Animals)
            .HasForeignKey(animal => animal.AnimalStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Volunteer>()
            .HasOne(volunteer => volunteer.VolunteerStatus)
            .WithMany(status => status.Volunteers)
            .HasForeignKey(volunteer => volunteer.VolunteerStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Donor>()
            .HasOne(donor => donor.DonorType)
            .WithMany(type => type.Donors)
            .HasForeignKey(donor => donor.DonorTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Donor>()
            .HasOne(donor => donor.DonorStatus)
            .WithMany(status => status.Donors)
            .HasForeignKey(donor => donor.DonorStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Employee>()
            .HasOne(employee => employee.EmployeePosition)
            .WithMany(position => position.Employees)
            .HasForeignKey(employee => employee.EmployeePositionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Employee>()
            .HasOne(employee => employee.EmployeeStatus)
            .WithMany(status => status.Employees)
            .HasForeignKey(employee => employee.EmployeeStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AnimalStatus>().HasData(
            new AnimalStatus { Id = 1, Name = "Dostupna za udomljenje" },
            new AnimalStatus { Id = 2, Name = "Rezervirana" },
            new AnimalStatus { Id = 3, Name = "Udomljena" },
            new AnimalStatus { Id = 4, Name = "Na liječenju" }
        );

        modelBuilder.Entity<VolunteerStatus>().HasData(
            new VolunteerStatus { Id = 1, Name = "Novi" },
            new VolunteerStatus { Id = 2, Name = "Aktivan" },
            new VolunteerStatus { Id = 3, Name = "Privremeno nedostupan" },
            new VolunteerStatus { Id = 4, Name = "Neaktivan" }
        );

        modelBuilder.Entity<DonorType>().HasData(
            new DonorType { Id = 1, Name = "Fizička osoba" },
            new DonorType { Id = 2, Name = "Tvrtka" },
            new DonorType { Id = 3, Name = "Udruga ili organizacija" }
        );

        modelBuilder.Entity<DonorStatus>().HasData(
            new DonorStatus { Id = 1, Name = "Novi" },
            new DonorStatus { Id = 2, Name = "Aktivan" },
            new DonorStatus { Id = 3, Name = "Povremeni" },
            new DonorStatus { Id = 4, Name = "Neaktivan" }
        );

        modelBuilder.Entity<EmployeePosition>().HasData(
            new EmployeePosition { Id = 1, Name = "Djelatnik azila" },
            new EmployeePosition { Id = 2, Name = "Veterinar" },
            new EmployeePosition { Id = 3, Name = "Koordinator volontera" },
            new EmployeePosition { Id = 4, Name = "Administrator" }
        );

        modelBuilder.Entity<EmployeeStatus>().HasData(
            new EmployeeStatus { Id = 1, Name = "Aktivan" },
            new EmployeeStatus { Id = 2, Name = "Na dopustu ili bolovanju" },
            new EmployeeStatus { Id = 3, Name = "Neaktivan" }
        );
    }
}

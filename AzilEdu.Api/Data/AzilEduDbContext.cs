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
    public DbSet<VolunteerTask> VolunteerTasks => Set<VolunteerTask>();
    public DbSet<VolunteerTaskStatus> VolunteerTaskStatuses => Set<VolunteerTaskStatus>();
    public DbSet<VolunteerTaskType> VolunteerTaskTypes => Set<VolunteerTaskType>();
    public DbSet<Donation> Donations => Set<Donation>();
    public DbSet<DonationType> DonationTypes => Set<DonationType>();
    public DbSet<DonationStatus> DonationStatuses => Set<DonationStatus>();
    public DbSet<AnimalMedia> AnimalMedia => Set<AnimalMedia>();
    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<AppRole> AppRoles => Set<AppRole>();
    public DbSet<AppUserRole> AppUserRoles => Set<AppUserRole>();
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

        modelBuilder.Entity<VolunteerTask>()
    .HasOne(task => task.Volunteer)
    .WithMany()
    .HasForeignKey(task => task.VolunteerId)
    .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<VolunteerTask>()
            .HasOne(task => task.Animal)
            .WithMany()
            .HasForeignKey(task => task.AnimalId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<VolunteerTask>()
            .HasOne(task => task.VolunteerTaskStatus)
            .WithMany(status => status.Tasks)
            .HasForeignKey(task => task.VolunteerTaskStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<VolunteerTask>()
            .HasOne(task => task.VolunteerTaskType)
            .WithMany(type => type.Tasks)
            .HasForeignKey(task => task.VolunteerTaskTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Donation>()
    .HasOne(donation => donation.Donor)
    .WithMany(donor => donor.Donations)
    .HasForeignKey(donation => donation.DonorId)
    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Donation>()
            .HasOne(donation => donation.DonationType)
            .WithMany(type => type.Donations)
            .HasForeignKey(donation => donation.DonationTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Donation>()
            .HasOne(donation => donation.DonationStatus)
            .WithMany(status => status.Donations)
            .HasForeignKey(donation => donation.DonationStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AnimalMedia>()
     .HasOne(media => media.Animal)
     .WithMany(animal => animal.Media)
     .HasForeignKey(media => media.AnimalId)
     .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AppUser>()
    .HasIndex(user => user.Email)
    .IsUnique();

        modelBuilder.Entity<AppUserRole>()
            .HasKey(userRole => new { userRole.AppUserId, userRole.AppRoleId });

        modelBuilder.Entity<AppUserRole>()
            .HasOne(userRole => userRole.AppUser)
            .WithMany(user => user.UserRoles)
            .HasForeignKey(userRole => userRole.AppUserId);

        modelBuilder.Entity<AppUserRole>()
            .HasOne(userRole => userRole.AppRole)
            .WithMany(role => role.UserRoles)
            .HasForeignKey(userRole => userRole.AppRoleId);

        modelBuilder.Entity<AppUser>()
            .HasOne(user => user.Donor)
            .WithMany()
            .HasForeignKey(user => user.DonorId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<AppUser>()
            .HasOne(user => user.Volunteer)
            .WithMany()
            .HasForeignKey(user => user.VolunteerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<AppUser>()
            .HasOne(user => user.Employee)
            .WithMany()
            .HasForeignKey(user => user.EmployeeId)
            .OnDelete(DeleteBehavior.SetNull);

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
        modelBuilder.Entity<VolunteerTaskStatus>().HasData(
    new VolunteerTaskStatus { Id = 1, Name = "Otvoren" },
    new VolunteerTaskStatus { Id = 2, Name = "Dodijeljen" },
    new VolunteerTaskStatus { Id = 3, Name = "U tijeku" },
    new VolunteerTaskStatus { Id = 4, Name = "Završeno" },
    new VolunteerTaskStatus { Id = 5, Name = "Otkazano" }
);

        modelBuilder.Entity<VolunteerTaskType>().HasData(
            new VolunteerTaskType { Id = 1, Name = "Šetnja" },
            new VolunteerTaskType { Id = 2, Name = "Hranjenje" },
            new VolunteerTaskType { Id = 3, Name = "Čišćenje" },
            new VolunteerTaskType { Id = 4, Name = "Socijalizacija" },
            new VolunteerTaskType { Id = 5, Name = "Prijevoz" },
            new VolunteerTaskType { Id = 6, Name = "Administracija" }
        );
        modelBuilder.Entity<DonationType>().HasData(
    new DonationType { Id = 1, Name = "Novčana" },
    new DonationType { Id = 2, Name = "Hrana" },
    new DonationType { Id = 3, Name = "Oprema" },
    new DonationType { Id = 4, Name = "Lijekovi" },
    new DonationType { Id = 5, Name = "Usluga" }
);

        modelBuilder.Entity<DonationStatus>().HasData(
            new DonationStatus { Id = 1, Name = "Evidentirana" },
            new DonationStatus { Id = 2, Name = "Potvrđena" },
            new DonationStatus { Id = 3, Name = "Iskorištena" },
            new DonationStatus { Id = 4, Name = "Otkazana" }
        );

        modelBuilder.Entity<AppRole>().HasData(
    new AppRole { Id = 1, Name = "User", DisplayName = "Korisnik" },
    new AppRole { Id = 2, Name = "Admin", DisplayName = "Administrator" },
    new AppRole { Id = 3, Name = "Employee", DisplayName = "Djelatnik" },
    new AppRole { Id = 4, Name = "Volunteer", DisplayName = "Volonter" },
    new AppRole { Id = 5, Name = "Donor", DisplayName = "Donator" }
);
    }
}

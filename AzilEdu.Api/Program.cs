using AzilEdu.Api.Data;
using AzilEdu.Api.Security;
using AzilEdu.Api.Services;
using AzilEdu.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AzilEduDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
var jwtOptions = jwtSection.Get<JwtOptions>()
    ?? throw new InvalidOperationException("Nedostaje Jwt konfiguracija.");

if (jwtOptions.SigningKey.Length < 32)
    throw new InvalidOperationException(
        "Jwt:SigningKey mora imati najmanje 32 znaka.");

builder.Services.Configure<JwtOptions>(jwtSection);
builder.Services.AddScoped<JwtTokenService>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.AddPolicy(
        AuthorizationPolicies.Staff,
        policy => policy.RequireRole("Admin", "Employee"));

    options.AddPolicy(
        AuthorizationPolicies.AdminOnly,
        policy => policy.RequireRole("Admin"));
});

builder.Services.Configure<AiOptions>(
    builder.Configuration.GetSection(AiOptions.SectionName));

builder.Services.AddScoped<MockAiService>();
builder.Services.AddScoped<OpenAiService>();

builder.Services.AddScoped<IAiService>(services =>
{
    var provider = builder.Configuration["Ai:Provider"];

    return string.Equals(
        provider,
        "OpenAI",
        StringComparison.OrdinalIgnoreCase)
            ? services.GetRequiredService<OpenAiService>()
            : services.GetRequiredService<MockAiService>();
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AzilEduDbContext>();

    await db.Database.MigrateAsync();

    if (!await db.Animals.AnyAsync())
    {
        db.Animals.AddRange(
            new Animal
            {
                Name = "Luna",
                Species = "Pas",
                Breed = "Labrador",
                Gender = "Ženka",
                Age = 3,
                ArrivalDate = new DateTime(2025, 10, 12),
                AnimalStatusId = 1,
                ImageUrl = "/images/animals/luna.webp",
                Description = "Mirna i druželjubiva kujica koja voli šetnje."
            },
            new Animal
            {
                Name = "Maza",
                Species = "Mačka",
                Breed = "Domaća kratkodlaka",
                Gender = "Ženka",
                Age = 2,
                ArrivalDate = new DateTime(2025, 11, 5),
                AnimalStatusId = 3,
                ImageUrl = "/images/animals/maza.webp",
                Description = "Zaigrana mačka naviknuta na boravak u zatvorenom prostoru."
            },
            new Animal
            {
                Name = "Rex",
                Species = "Pas",
                Breed = "Njemački ovčar",
                Gender = "Mužjak",
                Age = 5,
                ArrivalDate = new DateTime(2026, 1, 20),
                AnimalStatusId = 1,
                ImageUrl = "/images/animals/rex.webp",
                Description = "Aktivan pas koji traži iskusnijeg vlasnika."
            },
            new Animal
            {
                Name = "Nala",
                Species = "Mačka",
                Breed = "Maine Coon mješanac",
                Gender = "Ženka",
                Age = null,
                ArrivalDate = new DateTime(2026, 2, 3),
                AnimalStatusId = 2,
                ImageUrl = "/images/animals/nala.webp",
                Description = "Mlada mačka pronađena bez poznate povijesti."
            },
            new Animal
            {
                Name = "Tobi",
                Species = "Pas",
                Breed = "Mješanac",
                Gender = "Mužjak",
                Age = 1,
                ArrivalDate = null,
                AnimalStatusId = 4,
                ImageUrl = "/images/animals/tobi.webp",
                Description = "Vesel pas kojem datum dolaska još nije potvrđen."
            },
            new Animal
            {
                Name = "Bruno",
                Species = "Pas",
                Breed = "Bigl",
                Gender = "Mužjak",
                Age = 4,
                ArrivalDate = new DateTime(2025, 9, 18),
                AnimalStatusId = 3,
                ImageUrl = "/images/animals/bruno.webp",
                Description = "Udomljen pas koji ostaje u evidenciji azila."
            }
        );
    }

    if (!await db.Volunteers.AnyAsync())
    {
        db.Volunteers.AddRange(
            new Volunteer
            {
                FirstName = "Ana",
                LastName = "Horvat",
                Email = "ana.horvat@example.com",
                Phone = "091 111 2222",
                Skills = "Šetnja pasa, socijalizacija",
                AvailableFrom = new DateTime(2026, 7, 1),
                Notes = "Dostupna vikendom.",
                VolunteerStatusId = 2
            },
            new Volunteer
            {
                FirstName = "Marko",
                LastName = "Kovač",
                Email = "marko.kovac@example.com",
                Phone = "092 333 4444",
                Skills = "Prijevoz, pomoć kod veterinara",
                AvailableFrom = new DateTime(2026, 7, 10),
                Notes = "Ima vlastiti automobil.",
                VolunteerStatusId = 1
            }
        );
    }

    if (!await db.Donors.AnyAsync())
    {
        db.Donors.AddRange(
            new Donor
            {
                FirstName = "Ivana",
                LastName = "Babić",
                Email = "ivana.babic@example.com",
                Phone = "095 555 1212",
                Address = "Ulica donatora 1",
                City = "Osijek",
                Notes = "Donira hranu jednom mjesečno.",
                CreatedAt = new DateTime(2026, 6, 15),
                DonorTypeId = 1,
                DonorStatusId = 2
            },
            new Donor
            {
                OrganizationName = "Pet Plus d.o.o.",
                Email = "kontakt@petplus.example.com",
                Phone = "031 555 000",
                Address = "Industrijska 12",
                City = "Osijek",
                Notes = "Potencijalni donator opreme.",
                CreatedAt = new DateTime(2026, 6, 20),
                DonorTypeId = 2,
                DonorStatusId = 1
            }
        );
    }

    if (!await db.Employees.AnyAsync())
    {
        db.Employees.AddRange(
            new Employee
            {
                FirstName = "Petra",
                LastName = "Novak",
                Email = "petra.novak@aziledu.example.com",
                Phone = "031 100 200",
                EmployeeNumber = "EMP-001",
                HireDate = new DateTime(2025, 3, 1),
                Notes = "Koordinira raspored volontera.",
                EmployeePositionId = 3,
                EmployeeStatusId = 1
            },
            new Employee
            {
                FirstName = "Ivan",
                LastName = "Marić",
                Email = "ivan.maric@aziledu.example.com",
                Phone = "031 100 201",
                EmployeeNumber = "EMP-002",
                HireDate = new DateTime(2024, 9, 10),
                Notes = "Zadužen za svakodnevnu brigu o životinjama.",
                EmployeePositionId = 1,
                EmployeeStatusId = 1
            }
        );
    }

    await db.SaveChangesAsync();
    await AppUserSeeder.SeedAsync(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

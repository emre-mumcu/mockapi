// dotnet tool install --global dotnet-ef
// dotnet tool update --global dotnet-ef
// dotnet add package Microsoft.EntityFrameworkCore.Sqlite
// dotnet add package Microsoft.EntityFrameworkCore.Design
// dotnet ef migrations add Mig0 -o AppData/Migrations
// dotnet ef database update 

// dotnet add package Faker.Net
// dotnet add package NLipsum

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MockAPI.AppData.Entities;

namespace MockAPI.AppData;

public class AppDbContext : DbContext
{
    public AppDbContext() { }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // if (System.Diagnostics.Debugger.IsAttached == false) System.Diagnostics.Debugger.Launch();

        base.OnConfiguring(optionsBuilder);

        // optionsBuilder.EnableSensitiveDataLogging(false);

        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite(connectionString: App.Instance._DataConfiguration.GetSection("Database:ConnectionString").Value);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // if (System.Diagnostics.Debugger.IsAttached == false) System.Diagnostics.Debugger.Launch();

        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());

        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            // modelBuilder.SeedData();
        }
    }

    public virtual DbSet<ActionLog> ActionLogs => Set<ActionLog>();
    public virtual DbSet<Address> Addresses => Set<Address>();
    public virtual DbSet<City> Cities => Set<City>();
    public virtual DbSet<Company> Companies => Set<Company>();
    public virtual DbSet<County> Counties => Set<County>();
    public virtual DbSet<ExceptionLog> ExceptionLogs => Set<ExceptionLog>();
    public virtual DbSet<JsonData> JsonData => Set<JsonData>();
    public virtual DbSet<Role> Roles => Set<Role>();
    public virtual DbSet<State> States => Set<State>();
    public virtual DbSet<User> Users => Set<User>();
}

public static class DataSeeder
{
    /// <summary>
    /// This SeedData method will run along with migrations (add migration and update database).
    /// Primary key field (ID) data can be SET in this type of seeding.
    /// But this will cause the migration files to be bigger and migration times to be longer if the seed data size is large.
    /// </summary> 
    public static void SeedData(this ModelBuilder modelBuilder)
    {
        //
        // if (System.Diagnostics.Debugger.IsAttached == false) System.Diagnostics.Debugger.Launch();
        // 

        var data = ReadJsonData<List<JsonData>?>("uscities.json");

        if (data == null) return;

        modelBuilder.Entity<JsonData>().HasData(
            data.Select(d => new JsonData
            {
                Id = d.Id,
                City = d.City,
                City_ascii = d.City_ascii,
                State_id = d.State_id,
                State_name = d.State_name,
                County_fips = d.County_fips,
                County_name = d.County_name,
                Lat = d.Lat,
                Lng = d.Lng,
                Population = d.Population,
                Density = d.Density,
                Source = d.Source,
                Military = d.Military,
                Incorporated = d.Incorporated,
                Timezone = d.Timezone,
                Ranking = d.Ranking,
                Zips = d.Zips
            }).ToList()
        );



        // modelBuilder.Entity<Geo>().HasData( new Geo { Id=1, Lat=1.125F, Lng = -1.222F });
    }

    /// <summary>
    /// This SeedData method runs along with the application run.
    /// </summary>
    public static async Task SeedData(IServiceProvider services)
    {
        IWebHostEnvironment environment = services.GetRequiredService<IWebHostEnvironment>();

        if (environment.IsDevelopment())
        {
            IServiceScope scope = services.CreateScope();

            AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (context.Database.GetPendingMigrations().Any())
                await context.Database.MigrateAsync();
            else
                context.Database.EnsureCreated();

            var data = ReadJsonData<List<JsonData>?>("uscities.json");

            if (data == null) return;

            /// Normally the DbContext takes care of the transaction, but in this case (SET IDENTITY_INSERT) 
            /// manually taking care of the transactions are required. 
            /// Database context will generate a BEGIN TRAN after the SET IDENTITY_INSERT is issued.
            /// This will make transaction's inserts to fail since IDENTITY_INSERT seems to affect tables at session/transaction level.
            /// So, everything must be wrapped in a single transaction to work properly.
            {
                using var transaction = context.Database.BeginTransaction();

                // If single transactio is required
                // seed data here

                transaction.Commit();
            }

            if (!context.JsonData.Any())
            {
                var jsonData = data.Select(d => new JsonData
                {
                    Id = d.Id,
                    City = d.City,
                    City_ascii = d.City_ascii,
                    State_id = d.State_id,
                    State_name = d.State_name,
                    County_fips = d.County_fips,
                    County_name = d.County_name,
                    Lat = d.Lat,
                    Lng = d.Lng,
                    Population = d.Population,
                    Density = d.Density,
                    Source = d.Source,
                    Military = d.Military,
                    Incorporated = d.Incorporated,
                    Timezone = d.Timezone,
                    Ranking = d.Ranking,
                    Zips = d.Zips
                }).ToList();
                context.JsonData.AddRange(jsonData);
                await context.SaveChangesAsync();
            }

            if (!context.States.Any())
            {
                var stateData = data
                .GroupBy(i => new { i.State_id, i.State_name })
                .Select(d => new State
                {
                    StateId = d.Key.State_id,
                    StateName = d.Key.State_name
                }).ToList();
                context.States.AddRange(stateData);
                await context.SaveChangesAsync();
            }

            if (!context.Cities.Any())
            {
                var cityData = data
                .GroupBy(i => new { i.State_id, i.City_ascii })
                .Select(d => new City
                {
                    CityName = d.Key.City_ascii,
                    StateId = d.Key.State_id
                }).ToList();
                context.Cities.AddRange(cityData);
                await context.SaveChangesAsync();
            }

            if (!context.Counties.Any())
            {
                var jsonData = data.Select(d => new County
                {
                    DataId = Convert.ToInt32(d.Id),
                    CountyName = d.County_name,
                    StateId = d.State_id,
                    City = d.City_ascii,
                    Lat = d.Lat,
                    Lng = d.Lng,
                    Population = Convert.ToInt32(d.Population),
                    Density = Convert.ToSingle(d.Density),
                    Zips = d.Zips
                }).ToList();
                context.Counties.AddRange(jsonData);
                await context.SaveChangesAsync();
            }

            if (!context.Companies.Any())
            {
                try
                {
                    List<Company> companies = new List<Company>();

                    foreach (var i in Enumerable.Range(1, 100))
                    {
                        companies.Add(new Company
                        {
                            Name = $"{Faker.Company.Name()} {Faker.Company.Suffix()}",
                            CatchPhrase = Faker.Company.CatchPhrase(),
                            Bs = Faker.Company.BS()
                        });
                    }

                    context.Companies.AddRange(companies);

                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            if (!context.Addresses.Any())
            {
                try
                {
                    List<Address> addresses = new List<Address>();

                    foreach (var i in Enumerable.Range(1, 250))
                    {
                        addresses.Add(new Address
                        {
                            Street = Faker.Address.StreetAddress(),
                            Suite = Faker.Address.StreetName(),
                            County = context.Counties.OrderBy(c => EF.Functions.Random()).Take(1).First()
                        });
                    }

                    context.Addresses.AddRange(addresses);

                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            if (!context.Roles.Any())
            {
                try
                {
                    List<Role> roles = new List<Role> {
                        new Role { RoleCode="admin", RoleName="Administrator"},
                        new Role { RoleCode="user", RoleName="User"}
                    };

                    context.Roles.AddRange(roles);

                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            if (!context.Users.Any())
            {
                try
                {
                    List<User> users = new List<User>();

                    foreach (var i in Enumerable.Range(1, 10))
                    {
                        using var hmac = new HMACSHA512();

                        users.Add(new User
                        {
                            Name = Faker.Name.First(),
                            Surname = Faker.Name.Last(),
                            Username = Faker.Internet.UserName(),
                            Email = Faker.Internet.Email(),
                            PasswordHash = hmac.ComputeHash(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes("aA123456"))),
                            PasswordSalt = hmac.Key,
                            Roles = context.Roles.Where(r => r.RoleCode == "user").ToList(),
                            Address = context.Addresses.OrderBy(c => EF.Functions.Random()).Take(1).First(),
                            Phone = Faker.Phone.Number(),
                            WebSite = Faker.Internet.Url(),
                            Company = context.Companies.OrderBy(c => EF.Functions.Random()).Take(1).First()
                        });
                    }

                    context.Users.AddRange(users);

                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
    private static T? ReadJsonData<T>(string jsonPath)
    {
        T? data = JsonSerializer.Deserialize<T>(File.ReadAllText(jsonPath), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return data;
    }
}
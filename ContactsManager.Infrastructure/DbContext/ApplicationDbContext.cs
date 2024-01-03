using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.Domain.Entities.IdentityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace ContactsManager.Infrastructure.DbContext;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public virtual DbSet<Person> People { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
            modelBuilder.Entity<IdentityUserLogin<string>>(b =>
            {
                b.HasKey(l => new { l.LoginProvider, l.ProviderKey });
                b.ToTable("UserLogins");
            });
            
            modelBuilder.Entity<IdentityUserRole<string>>(b =>
            {
                b.HasKey(r => new { r.UserId, r.RoleId });
                b.ToTable("UserRoles");
            });
            
            modelBuilder.Entity<IdentityUserToken<string>>(b =>
            {
                b.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });
                b.ToTable("UserTokens");
            });
            
        
            modelBuilder.Entity<Country>().ToTable("Countries");
            
            modelBuilder.Entity<Person>()
                .ToTable("People", tb => 
                    tb.HasCheckConstraint("CHK_TIN", "CHAR_LENGTH(TaxIdentificationNumber) = 8"));
        
        
            modelBuilder.Entity<Person>()
                .Property(r => r.PersonId)
                .ValueGeneratedOnAdd();
        
            modelBuilder.Entity<Country>()
                .Property(r => r.CountryId)
                .ValueGeneratedOnAdd();
        
            // //Seed to Countries Table
            // string countriesJson = File.ReadAllText("countries.json");
            //
            // List<Country> countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesJson);
            //
            // foreach (Country country in countries)
            // {
            //     modelBuilder.Entity<Country>().HasData(country);
            // }
            //
            // string personJson = File.ReadAllText("persons.json");
            //
            // List<Person> people = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personJson);
            //
            // foreach (Person person in people)
            // {
            //     modelBuilder.Entity<Person>().HasData(person);
            // }
        
            // //Fluent API
            // modelBuilder.Entity<Person>().Property(p => p.TIN)
            //     .HasColumnName("TaxIdentificationNumber")
            //     .HasColumnType("varchar(8)");
        
            // modelBuilder.Entity<Person>().HasIndex(p => p.TIN).IsUnique();
        
            // modelBuilder.Entity<Person>(entity => entity.HasOne<Country>(c => c.Country)
            //     .WithMany(p => p.People)
            //     .HasForeignKey(p => p.CountryId));
        }
        
        public List<Person> sp_GetAllPeople()
        {
            return People.FromSqlRaw("CALL GetAllPeople").ToList();
        }
        
        public int sp_AddPerson(Person person)
        {
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("p_PersonId", MySqlDbType.Int32) { Value = person.PersonId },
                new MySqlParameter("p_PersonName", MySqlDbType.VarChar) { Value = person.PersonName },
                new MySqlParameter("p_Email", MySqlDbType.VarChar) { Value = person.Email },
                new MySqlParameter("p_DateOfBirth", MySqlDbType.DateTime) { Value = person.DateOfBirth },
     
                new MySqlParameter("p_CountryId", MySqlDbType.Int32) { Value = person.CountryId },
                new MySqlParameter("p_Country", MySqlDbType.VarChar) { Value = person.Country?.CountryName },
                new MySqlParameter("p_Address", MySqlDbType.VarChar) { Value = person.Address },
          
            };
        
            return Database.ExecuteSqlRaw("CALL AddPerson(?, ?, ?, ?, ?, ?, ?, ?, ?)", parameters);
    }
}

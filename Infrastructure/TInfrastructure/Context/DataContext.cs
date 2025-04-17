using Core.Model;
using Microsoft.EntityFrameworkCore;

namespace TInfrastructure.Context
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<WeatherForecast> WeatherForecasts { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Coach> Coaches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Explicit table names to match SQL schema
            modelBuilder.Entity<Account>().ToTable("Account");
            modelBuilder.Entity<Person>().ToTable("Person");
            modelBuilder.Entity<WeatherForecast>().ToTable("WeatherForecast");

            // TPH inheritance (one table: Person)
            modelBuilder.Entity<Person>()
                .HasDiscriminator<string>("PersonType")
                .HasValue<Person>("Person")
                .HasValue<Driver>("Driver")
                .HasValue<Coach>("Coach");

            // One-to-one: Account ↔ Person
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Person)
                .WithOne(p => p.Account)
                .HasForeignKey<Person>(p => p.AccountID)
                .OnDelete(DeleteBehavior.Cascade);

            // Date column mapping for WeatherForecast
            modelBuilder.Entity<WeatherForecast>()
                .Property(w => w.Date)
                .HasColumnType("DATE");

            // Optional: Add constraints
            modelBuilder.Entity<Account>()
                .Property(a => a.Email)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<Account>()
                .Property(a => a.Username)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Account>()
                .Property(a => a.PasswordHash)
                .IsRequired();

            modelBuilder.Entity<Person>()
                .Property(p => p.FirstName)
                .HasMaxLength(100);

            modelBuilder.Entity<Person>()
                .Property(p => p.Prefix)
                .HasMaxLength(20);

            modelBuilder.Entity<Person>()
                .Property(p => p.LastName)
                .HasMaxLength(100);
        }
    }
}
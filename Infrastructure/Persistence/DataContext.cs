using Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Coach> Coaches { get; set; }

        public DbSet<Circuit> Circuits { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Heat> Heats { get; set; }
        public DbSet<LapTime> LapTimes { get; set; }
        public DbSet<MiniSector> MiniSectors { get; set; }
        public DbSet<GPSPoint> GPSPoints { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Explicit table names
            modelBuilder.Entity<Account>().ToTable("Account");
            modelBuilder.Entity<Person>().ToTable("Person");
            modelBuilder.Entity<Circuit>().ToTable("Circuit");
            modelBuilder.Entity<Session>().ToTable("Session");
            modelBuilder.Entity<Heat>().ToTable("Heat");
            modelBuilder.Entity<LapTime>().ToTable("LapTime");

            // Inheritance
            modelBuilder.Entity<Person>()
                .HasDiscriminator<string>("PersonType")
                .HasValue<Person>("Person")
                .HasValue<Driver>("Driver")
                .HasValue<Coach>("Coach");

            // Account ↔ Person
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Person)
                .WithOne(p => p.Account)
                .HasForeignKey<Person>(p => p.AccountID)
                .OnDelete(DeleteBehavior.Cascade);

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
                .Property(p => p.FirstName).HasMaxLength(100);
            modelBuilder.Entity<Person>()
                .Property(p => p.Prefix).HasMaxLength(20);
            modelBuilder.Entity<Person>()
                .Property(p => p.LastName).HasMaxLength(100);

            // Session ↔ Circuit
            modelBuilder.Entity<Session>()
                .HasOne(s => s.Circuit)
                .WithMany() // No navigation back from Circuit to Sessions
                .HasForeignKey(s => s.CircuitID)
                .OnDelete(DeleteBehavior.Restrict);

            // Session ↔ Heat
            modelBuilder.Entity<Session>()
                .HasMany(s => s.Heats)
                .WithOne(h => h.Session)
                .HasForeignKey(h => h.SessionID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Session>()
                .HasOne(s => s.Account)
                .WithMany()
                .HasForeignKey(s => s.AccountID)
                .OnDelete(DeleteBehavior.Cascade);


            // Heat ↔ LapTime
            modelBuilder.Entity<Heat>()
                .HasMany(h => h.LapTimes)
                .WithOne(lt => lt.Heat)
                .HasForeignKey(lt => lt.HeatID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MiniSector>()
                .HasOne(ms => ms.LapTime)
                .WithMany(lt => lt.MiniSectors)
                .HasForeignKey(ms => ms.LapTimeID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GPSPoint>()
                .HasOne(g => g.LapTime)
                .WithMany(lt => lt.GPSPoints)
                .HasForeignKey(g => g.LapTimeID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
using ContractSystem_Blogic.Models;
using Microsoft.EntityFrameworkCore;

namespace ContractSystem_Blogic.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Consultant> Consultants { get; set; }
        public DbSet<Institution> Institutions { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contract>()
                .HasOne(c => c.ContractManager)
                .WithMany()
                .HasForeignKey(c => c.ContractManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Contract>()
                .HasMany(c => c.Consultants)
                .WithMany(c => c.Contracts)
                .UsingEntity<Dictionary<string, object>>(
                    "ContractConsultant",
                    j => j.HasOne<Consultant>().WithMany().HasForeignKey("ConsultantId").OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<Contract>().WithMany().HasForeignKey("ContractId").OnDelete(DeleteBehavior.Cascade)
                );
        }
    }
}

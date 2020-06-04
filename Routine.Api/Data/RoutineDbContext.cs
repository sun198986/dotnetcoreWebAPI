using System;
using Microsoft.EntityFrameworkCore;
using Routine.Api.Entities;

namespace Routine.Api.Data
{
    public class RoutineDbContext : DbContext
    {
        public RoutineDbContext(DbContextOptions<RoutineDbContext> options) : base(options)
        {

        }

        public DbSet<Company> Companies { get; set; }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>().Property(x => x.Name).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Company>().Property(x => x.Introduction).IsRequired().HasMaxLength(500);

            modelBuilder.Entity<Employee>().Property(x => x.EmployeeNo).IsRequired().HasMaxLength(10);
            modelBuilder.Entity<Employee>().Property(x => x.FirstName).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Employee>().Property(x => x.LastName).IsRequired().HasMaxLength(50);

            modelBuilder.Entity<Employee>().HasOne(x => x.Company)
                .WithMany(x => x.Employees)
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Company>().HasData(new Company
            {

                Id = Guid.Parse("0d8c6be7-e984-4ade-8997-8a1e67ea0c22"),
                Name = "Microsoft",
                Introduction = "Great Company"
            }, new Company
            {

                Id = Guid.Parse("6fa484b4-26aa-405a-a1f6-7f82092f66b6"),
                Name = "Google",
                Introduction = "Don't be evil"
            }, new Company
            {

                Id = Guid.Parse("658d4ed3-1505-4d02-b9ed-942e2a918e38"),
                Name = "Ali papa",
                Introduction = "Zz Company"
            });
        }
    }
}
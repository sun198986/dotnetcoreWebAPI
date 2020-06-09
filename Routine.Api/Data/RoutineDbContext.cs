using System;
using System.Collections.Generic;
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
            modelBuilder.Entity<Company>().Property(x => x.Country).HasMaxLength(50);
            modelBuilder.Entity<Company>().Property(x => x.Industry).HasMaxLength(50);
            modelBuilder.Entity<Company>().Property(x => x.Product).HasMaxLength(100);
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
                Country = "USA",
                Industry = "Software",
                Product = "Software",
                Introduction = "Great Company"
            }, new Company
            {

                Id = Guid.Parse("6fa484b4-26aa-405a-a1f6-7f82092f66b6"),
                Name = "Google",
                Country = "USA",
                Industry = "Internet",
                Product = "Software",
                Introduction = "Don't be evil"
            }, new Company
            {
                Id = Guid.Parse("658d4ed3-1505-4d02-b9ed-942e2a918e38"),
                Name = "Ali papa",
                Country = "China",
                Industry = "Internet",
                Product = "Software",
                Introduction = "Zz Company"
            });

            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = Guid.Parse("e56d4dcc-313b-4310-928f-01cc293fd7da"),
                    CompanyId = Guid.Parse("0d8c6be7-e984-4ade-8997-8a1e67ea0c22"),
                    DateOfBirth = new DateTime(1986, 11, 4),
                    EmployeeNo = "G055",
                    FirstName = "Harry",
                    LastName = "Miko",
                    Gender = Gender.男
                },
                new Employee
                {
                    Id = Guid.Parse("a68c7721-beb7-453d-b9f1-a661a2040ed4"),
                    CompanyId = Guid.Parse("0d8c6be7-e984-4ade-8997-8a1e67ea0c22"),
                    DateOfBirth = new DateTime(1976, 5, 6),
                    EmployeeNo = "G001",
                    FirstName = "Live",
                    LastName = "Mai",
                    Gender = Gender.男
                }
            );
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = Guid.Parse("2affcefc-9ae1-4bd5-bb6e-6100ab0b4faa"),
                    CompanyId = Guid.Parse("6fa484b4-26aa-405a-a1f6-7f82092f66b6"),
                    DateOfBirth = new DateTime(1986, 11, 4),
                    EmployeeNo = "G016",
                    FirstName = "Love",
                    LastName = "Pi",
                    Gender = Gender.女
                },
                new Employee
                {
                    Id = Guid.Parse("124bcc74-7bc5-4a25-ad43-e43814014ef9"),
                    CompanyId = Guid.Parse("6fa484b4-26aa-405a-a1f6-7f82092f66b6"),
                    DateOfBirth = new DateTime(1976, 5, 6),
                    EmployeeNo = "G044",
                    FirstName = "Papa",
                    LastName = "Richardson",
                    Gender = Gender.女
                }
            );
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = Guid.Parse("267b4f39-1641-4387-9e2d-584f5fec4bfd"),
                    CompanyId = Guid.Parse("658d4ed3-1505-4d02-b9ed-942e2a918e38"),
                    DateOfBirth = new DateTime(1986, 11, 4),
                    EmployeeNo = "G003",
                    FirstName = "Marry",
                    LastName = "King",
                    Gender = Gender.女
                },
                new Employee
                {
                    Id = Guid.Parse("4d143264-be9a-41e7-83d2-a6bd5a0c7e7d"),
                    CompanyId = Guid.Parse("658d4ed3-1505-4d02-b9ed-942e2a918e38"),
                    DateOfBirth = new DateTime(1976, 5, 6),
                    EmployeeNo = "G004",
                    FirstName = "Kevin",
                    LastName = "Richardson",
                    Gender = Gender.男
                }
            );
        }
    }
}
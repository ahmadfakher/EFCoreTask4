using EFCoreTask4.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreTask4.DbContexts
{
    internal class BankDbContext : DbContext
    {
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<CustomerAccount> CustomerAccounts { get; set; }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=.;Database=NationalBankDB;Trusted_Connection=True;TrustServerCertificate=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Branch>(entity =>
            {
                entity.HasKey(b => b.Code);

                entity.Property(b => b.Code)
                      .HasMaxLength(10);

                entity.Property(b => b.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(b => b.Address)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(b => b.PhoneNumber)
                      .IsRequired()
                      .HasMaxLength(20);
            });

            modelBuilder.Entity<Manager>(entity =>
            {
                entity.HasKey(m => m.Id);

                entity.Property(m => m.FullName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(m => m.Email)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.HasOne(m => m.Branch)
                      .WithOne(b => b.Manager)
                      .HasForeignKey<Manager>(m => m.BranchCode);
            });

            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(a => a.AccountNumber);

                entity.Property(a => a.AccountNumber)
                      .HasMaxLength(20);

                entity.Property(a => a.CurrentBalance)
                      .HasColumnType("decimal(18,2)");

                entity.HasOne(a => a.Branch)
                      .WithMany(b => b.Accounts)
                      .HasForeignKey(a => a.BranchCode);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.FullName)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(c => c.NationalId)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.HasIndex(c => c.NationalId)
                      .IsUnique();
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("Transactions");

                entity.HasKey(t => t.TransactionNumber);

                entity.Property(t => t.Amount)
                      .HasColumnType("decimal(18,2)");

                entity.HasOne(t => t.Account)
                      .WithMany(a => a.Transactions)
                      .HasForeignKey(t => t.AccountNumber);
            });

            modelBuilder.Entity<CustomerAccount>(entity =>
            {
                entity.HasKey(ca => new
                {
                    ca.CustomerId,
                    ca.AccountNumber
                });

                entity.HasOne(ca => ca.Customer)
                      .WithMany(c => c.CustomerAccounts)
                      .HasForeignKey(ca => ca.CustomerId);

                entity.HasOne(ca => ca.Account)
                      .WithMany(a => a.CustomerAccounts)
                      .HasForeignKey(ca => ca.AccountNumber);
            });

            
            modelBuilder.Entity<Branch>().HasData(
                new Branch
                {
                    Code = "CAI01",
                    Name = "Cairo Branch",
                    Address = "Cairo, Egypt",
                    PhoneNumber = "01012345678"
                },
                new Branch
                {
                    Code = "ALX01",
                    Name = "Alexandria Branch",
                    Address = "Alexandria, Egypt",
                    PhoneNumber = "01098765432"
                }
            );

            
            modelBuilder.Entity<Manager>().HasData(
                new Manager
                {
                    Id = 1,
                    FullName = "Ahmed Hassan",
                    Email = "ahmed@gmail.com",
                    PhoneNumber = "01112345678",
                    HireDate = new DateTime(2020, 1, 1),
                    BranchCode = "CAI01"
                },
                new Manager
                {
                    Id = 2,
                    FullName = "Mohamed Ali",
                    Email = "mohamed@gmail.com",
                    PhoneNumber = "01198765432",
                    HireDate = new DateTime(2021, 5, 1),
                    BranchCode = "ALX01"
                }
            );
        }
    }
}

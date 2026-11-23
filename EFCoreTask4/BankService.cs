using EFCoreTask4.DbContexts;
using EFCoreTask4.Enums;
using EFCoreTask4.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreTask4
{
    internal class BankService
    {
        public int AddCustomer(Customer customer)
        {
            using var context = new BankDbContext();

            context.Customers.Add(customer);

            context.SaveChanges();

            return customer.Id;
        }

        public bool OpenAccount(string accountNumber, AccountType accountType, string branchCode, int customerId, OwnershipType ownershipType)
        {
            using var context = new BankDbContext();

            bool branchExists = context.Branches
                .Any(b => b.Code == branchCode);

            bool customerExists = context.Customers
                .Any(c => c.Id == customerId);

            if (!branchExists || !customerExists)
                return false;

            bool accountExists = context.Accounts
                .Any(a => a.AccountNumber == accountNumber);

            if (accountExists)
                return false;

            var account = new Account
            {
                AccountNumber = accountNumber,
                AccountType = accountType,
                BranchCode = branchCode,
                CurrentBalance = 0,
                OpeningDate = DateTime.Now,
                AccountStatus = AccountStatus.Active
            };

            var customerAccount = new CustomerAccount
            {
                AccountNumber = accountNumber,
                CustomerId = customerId,
                OwnershipType = ownershipType,
                OwnershipStartDate = DateTime.Now
            };

            context.Accounts.Add(account);
            context.CustomerAccounts.Add(customerAccount);

            context.SaveChanges();

            return true;
        }

        public bool UpdateAccountStatus(string accountNumber, int customerId, AccountStatus newStatus)
        {
            using var context = new BankDbContext();

            var customerAccount = context.CustomerAccounts
                .Include(ca => ca.Account)
                .FirstOrDefault(ca =>
                    ca.AccountNumber == accountNumber &&
                    ca.CustomerId == customerId);

            if (customerAccount == null)
                return false;

            customerAccount.Account.AccountStatus = newStatus;

            context.SaveChanges();

            return true;
        }

        public bool RemoveAccountFromCustomer(string accountNumber, int customerId)
        {
            using var context = new BankDbContext();

            var customerAccount = context.CustomerAccounts
                .FirstOrDefault(ca =>
                    ca.AccountNumber == accountNumber &&
                    ca.CustomerId == customerId);

            if (customerAccount == null)
                return false;

            context.CustomerAccounts.Remove(customerAccount);

            context.SaveChanges();

            bool hasOtherOwners = context.CustomerAccounts
                .Any(ca => ca.AccountNumber == accountNumber);

            if (!hasOtherOwners)
            {
                var account = context.Accounts
                    .FirstOrDefault(a => a.AccountNumber == accountNumber);

                if (account != null)
                {
                    context.Accounts.Remove(account);
                    context.SaveChanges();
                }
            }

            return true;
        }

        public void ListCustomers()
        {
            using var context = new BankDbContext();

            var customers = context.Customers
                .Include(c => c.CustomerAccounts)
                    .ThenInclude(ca => ca.Account)
                        .ThenInclude(a => a.Branch)
                .ToList();

            foreach (var customer in customers)
            {
                Console.WriteLine(
                    $"#{customer.Id} {customer.FullName} ({customer.CustomerType})");

                if (!customer.CustomerAccounts.Any())
                {
                    Console.WriteLine("    (no accounts)");
                    continue;
                }

                foreach (var customerAccount in customer.CustomerAccounts)
                {
                    var account = customerAccount.Account;

                    Console.WriteLine(
                        $"{account.AccountNumber}  " +
                        $"{account.AccountType}  " +
                        $"Balance: {account.CurrentBalance:N2}  " +
                        $"{customerAccount.OwnershipType}  " +
                        $"{account.AccountStatus}  " +
                        $"@ {account.Branch.Name}");
                }
            }
        }
    }
}

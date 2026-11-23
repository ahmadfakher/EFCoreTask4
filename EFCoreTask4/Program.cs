using EFCoreTask4.Enums;
using EFCoreTask4.Models;
using System;

namespace EFCoreTask4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var service = new BankService();

            bool running = true;

            while (running)
            {
                Console.Clear();

                Console.WriteLine("======================================");
                Console.WriteLine("       Bank - Management");
                Console.WriteLine("======================================");
                Console.WriteLine(" 1) Add a new Customer");
                Console.WriteLine(" 2) Open a new Account for a Customer");
                Console.WriteLine(" 3) Update Account Status");
                Console.WriteLine(" 4) Remove an Account from a Customer");
                Console.WriteLine(" 5) List all Customers (with accounts)");
                Console.WriteLine(" 0) Exit");
                Console.WriteLine("--------------------------------------");

                int choice = ReadInt("Enter choice: ");

                try
                {
                    switch (choice)
                    {
                        case 1:
                            AddCustomer(service);
                            break;

                        case 2:
                            OpenAccount(service);
                            break;

                        case 3:
                            UpdateStatus(service);
                            break;

                        case 4:
                            RemoveAccount(service);
                            break;

                        case 5:
                            service.ListCustomers();
                            break;

                        case 0:
                            running = false;
                            Console.WriteLine("Goodbye!");
                            continue;

                        default:
                            Console.WriteLine("Invalid choice. Try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Operation failed: {ex.Message}");
                }

                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
            }
        }

        static int ReadInt(string message)
        {
            while (true)
            {
                Console.Write(message);

                if (int.TryParse(Console.ReadLine(), out int value))
                    return value;

                Console.WriteLine("Invalid number. Please try again.");
            }
        }

        static string ReadRequiredString(string message)
        {
            while (true)
            {
                Console.Write(message);

                string value = Console.ReadLine()?.Trim() ?? "";

                if (!string.IsNullOrWhiteSpace(value))
                    return value;

                Console.WriteLine("This field is required.");
            }
        }

        static DateTime ReadDate(string message)
        {
            while (true)
            {
                Console.Write(message);

                if (DateTime.TryParse(Console.ReadLine(), out DateTime date))
                    return date;

                Console.WriteLine("Invalid date. Please try again.");
            }
        }

        static T ReadEnum<T>(string message) where T : struct, Enum
        {
            while (true)
            {
                Console.Write(message);

                if (int.TryParse(Console.ReadLine(), out int value) &&
                    Enum.IsDefined(typeof(T), value))
                {
                    return (T)Enum.ToObject(typeof(T), value);
                }

                Console.WriteLine("Invalid choice. Please try again.");
            }
        }

        static void AddCustomer(BankService service)
        {
            Console.WriteLine("\n--- Add New Customer ---");

            var customer = new Customer
            {
                FullName = ReadRequiredString("Full Name: "),
                NationalId = ReadRequiredString("National ID: "),
                DateOfBirth = ReadDate("Date of Birth: "),
                Email = ReadRequiredString("Email: "),
                PhoneNumber = ReadRequiredString("Phone: "),
                Address = ReadRequiredString("Address: "),
                CustomerType = ReadEnum<CustomerType>(
                    "Customer Type:\n1) Individual\n2) Business\nChoice: ")
            };

            int id = service.AddCustomer(customer);

            Console.WriteLine(
                $"Customer created successfully. CustomerId = {id}");
        }

        static void OpenAccount(BankService service)
        {
            Console.WriteLine("\n--- Open New Account ---");

            string accountNumber =
                ReadRequiredString("Account Number: ");

            Console.WriteLine(
                "Account Type:\n1) Savings\n2) Current\n3) Business");

            var accountType = ReadEnum<AccountType>("Choice: ");

            string branchCode =
                ReadRequiredString("Branch Code: ");

            int customerId =
                ReadInt("Customer Id: ");

            Console.WriteLine(
                "Ownership Role:\n1) Primary\n2) CoHolder");

            var ownershipType =
                ReadEnum<OwnershipType>("Choice: ");

            Console.WriteLine(
                $"Validating branch '{branchCode}' and customer #{customerId}...");

            bool result = service.OpenAccount(
                accountNumber,
                accountType,
                branchCode,
                customerId,
                ownershipType);

            if (result)
            {
                Console.WriteLine(
                    $"Account '{accountNumber}' created and linked " +
                    $"to customer {customerId} as {ownershipType} owner.");
            }
            else
            {
                Console.WriteLine(
                    "Account creation failed. Check the branch, customer, " +
                    "or account number.");
            }
        }

        static void UpdateStatus(BankService service)
        {
            Console.WriteLine("\n--- Update Account Status ---");

            string accountNumber =
                ReadRequiredString("Account Number: ");

            int customerId =
                ReadInt("Customer Id: ");

            Console.WriteLine(
                "New Status:\n1) Active\n2) Closed");

            var status = ReadEnum<AccountStatus>("Choice: ");

            bool result = service.UpdateAccountStatus(
                accountNumber,
                customerId,
                status);

            if (result)
                Console.WriteLine($"Status updated to {status}.");
            else
                Console.WriteLine("Account or customer relationship not found.");
        }

        static void RemoveAccount(BankService service)
        {
            Console.WriteLine("\n--- Remove Account From Customer ---");

            string accountNumber =
                ReadRequiredString("Account Number: ");

            int customerId =
                ReadInt("Customer Id: ");

            bool result = service.RemoveAccountFromCustomer(
                accountNumber,
                customerId);

            if (result)
                Console.WriteLine("Ownership link deleted.");
            else
                Console.WriteLine("Account/customer relationship not found.");
        }
    }
}

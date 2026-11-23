using EFCoreTask4.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreTask4.Models
{
    internal class Account
    {
        public string AccountNumber { get; set; }

        public decimal CurrentBalance { get; set; }

        public AccountType AccountType { get; set; }

        public DateTime OpeningDate { get; set; }

        public AccountStatus AccountStatus { get; set; }

        
        public string BranchCode { get; set; }

        public Branch Branch { get; set; }

        
        public ICollection<CustomerAccount> CustomerAccounts { get; set; }
            = new List<CustomerAccount>();

        
        public ICollection<Transaction> Transactions { get; set; }
            = new List<Transaction>();
    }
}

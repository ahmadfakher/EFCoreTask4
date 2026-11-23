using EFCoreTask4.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreTask4.Models
{
    internal class CustomerAccount
    {
        public int CustomerId { get; set; }

        public string AccountNumber { get; set; }

        public OwnershipType OwnershipType { get; set; }

        public DateTime OwnershipStartDate { get; set; }

        public Customer Customer { get; set; }

        public Account Account { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestBankingService.Implementation
{
    public class Account
    {
        public int Id { get; set; }
        public int AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}

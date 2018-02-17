
using RestBankingConsoleClient.AcountService;
using System;
using System.Collections.Generic;
using System.Linq;


namespace RestBankingConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new AccountServiceClient();



            var request = new RestBankingClient.RestBankingService.AccountRequest() { AccountNumber = 1234, Amount = 40, Currency = "USD" };
            

            var response=service.Withdraw(request);
            Console.ReadLine();
        }
    }
}

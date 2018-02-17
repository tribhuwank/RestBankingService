using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace RestBankingService.SelfHost
{
    class Program
    {
        static void Main()
        {
            using (var host = new ServiceHost(typeof(Implementation.AccountService)))
            {
                host.Open();
                Console.WriteLine("Service is started at http://localhost/api/account/");
                Console.ReadLine();
            }
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestBankingService.Contract;

namespace RestBankingService.Test
{
    [TestClass]
    public class AccountServiceTest
    {
        [TestMethod]
        public void Test_Deposit()
        {
            //Arrage
            //Act
            //Assert

        }
        [TestMethod]
        public void Test_Withdraw()
        {
            //Arrage
            int accountNumber = 1234;
            decimal actualValue = 1000;
            string currency = "USD";
            var req = new AccountRequest {AccountNumber= accountNumber,Amount=40,Currency= currency };
            //Act
            var service = new Implementation.AccountService();
            var result = service.Withdraw(req);
            //Assert
            Assert.AreEqual(actualValue, result);

        }
        [TestMethod]
        public void Test_Balance()
        {
            //Arrage
            int accountNumber = 1234;
            decimal actualValue = 500;
            //Act
            var service = new Implementation.AccountService();
            var result=service.Balance(accountNumber);

            //Assert
            Assert.AreEqual(actualValue, result);
        }
    }
}

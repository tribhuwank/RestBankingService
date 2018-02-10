using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            //Act
            //Assert

        }
        [TestMethod]
        public void Test_Balance()
        {
            //Arrage
            int accountNumber = 1234;
            decimal actualValue = 0;
            //Act
            var service = new Implementation.AccountService();
            var result=service.Balance(accountNumber);

            //Assert
            Assert.AreEqual(actualValue, result);
        }
    }
}

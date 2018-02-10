using RestBankingService.Contract;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.ServiceModel;

namespace RestBankingService.Implementation
{
    [ServiceBehavior(IncludeExceptionDetailInFaults =true,ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession)]
    public class AccountService : IAccountService
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
        public AccountResponse Balance(int accountNumber)
        {
            var response = new AccountResponse { AccountNumber = accountNumber };

            using (var connection =
            new SqlConnection(connectionString))
            {

                var command = new SqlCommand("SP_DEPOSITE_AMOUNT;", connection) { CommandType = System.Data.CommandType.StoredProcedure };
                command.Parameters.AddWithValue("@accountNumber", accountNumber);
                try
                {

                    connection.Open();
                    var result = command.ExecuteScalar();
                    response.Successful = true;
                }
                catch (Exception)
                {
                    response.Successful = false;
                }

            }

            return response;
        }

        public AccountResponse Deposit(AccountRequest request)
        {
            var response = new AccountResponse { AccountNumber = request.AccountNumber };

            using (var connection =
            new SqlConnection(connectionString))
            {

                var command = new SqlCommand("SP_DEPOSITE_AMOUNT;", connection) { CommandType = System.Data.CommandType.StoredProcedure };
                var param = new SqlParameter() { Direction = System.Data.ParameterDirection.Output, ParameterName = "@Balance" };
                command.Parameters.Add(param);
                command.Parameters.AddWithValue("@accountNumber", request.AccountNumber);
                command.Parameters.AddWithValue("@Currency", request.Currency);
                command.Parameters.AddWithValue("@Amount", request.Amount);
                try
                {

                    connection.Open();
                    var result = command.ExecuteNonQuery();
                    response.Successful = true;
                }
                catch (Exception )
                {
                    response.Successful = false;
                }

            }

            if (response.Successful)
                response.Message = "Deposit Transaction is done SuccessFully";
            else
                response.Message = "Deposit Transaction is done UnSuccessFully";
            return response;
        }

        public AccountResponse Withdraw(AccountRequest request)
        {
            var response = new AccountResponse { AccountNumber = request.AccountNumber };

            using (var connection =
            new SqlConnection(connectionString))
            {

                var command = new SqlCommand("SP_WITHDRAW_AMOUNT;", connection) { CommandType = System.Data.CommandType.StoredProcedure };
                var param = new SqlParameter() { Direction = System.Data.ParameterDirection.Output, ParameterName = "@Balance" };
                command.Parameters.Add(param);
                command.Parameters.AddWithValue("@accountNumber", request.AccountNumber);
                command.Parameters.AddWithValue("@Currency", request.Currency);
                command.Parameters.AddWithValue("@Amount", request.Amount);
                try
                {

                    connection.Open();
                    var result = command.ExecuteNonQuery();
                    response.Successful = true;
                   

                }
                catch (Exception )
                {
                    response.Successful = false;
                }

            }
            if(response.Successful)
                response.Message = "Withdraw Transaction is done SuccessFully";
            else
                response.Message = "Withdraw Transaction is done UnSuccessFully";
            return response;
        }
    }
}

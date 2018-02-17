using RestBankingService.Contract;
using System;
using System.Data.SqlClient;
using System.ServiceModel;
using System.Threading.Tasks;

namespace RestBankingService.Implementation
{
    /// <summary>
    /// AccountService
    /// </summary>
    [ServiceBehavior(IncludeExceptionDetailInFaults =true,ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession)]
    public class AccountService : IAccountService
    {
        private static int _accountNumber;
        private static decimal _balance=-1;
        private string _connectionString;
        
        private static Account CurrentAccount;
        /// <summary>
        /// getAccount
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        public Account getAccount(int accountNumber)
        {
            _accountNumber = accountNumber;
            if (!(_accountNumber <= 0))
            {
                CurrentAccount = new Account();
                using (var connection =new SqlConnection(ConnectionString))
                {
                    var command = new SqlCommand("[SP_GetAccount]", connection) { CommandType = System.Data.CommandType.StoredProcedure };
                    command.Parameters.Add(new SqlParameter() {Direction=System.Data.ParameterDirection.Input,ParameterName= "@AccountNumber",Value=accountNumber });
                    try
                    {

                        connection.Open();
                       var reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            while (reader.HasRows)
                            {
                                CurrentAccount.Id= (int)reader["Id"];
                                CurrentAccount.AccountNumber = (int)reader["AccountNumber"];
                                CurrentAccount.Currency= reader["Currency"].ToString();
                                CurrentAccount.Amount= (decimal)reader["Amount"];
                                reader.NextResult();
                            }
                           
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }                    
            }
            _balance= CurrentAccount.Amount;
            return CurrentAccount;
        }

        public string ConnectionString { get => _connectionString; set => _connectionString = value; }

        /// <summary>
        /// constructor AccountService
        /// </summary>
        public AccountService()
        {
            ConnectionString = "server=Jaychand-PC;database=BankDB;uid=sa;password=sa;";
        }

        /// <summary>
        /// GetCurrentExchangeRateAsync
        /// </summary>
        /// <param name="baseCurrency"></param>
        /// <returns></returns>
        private async Task<TodayRates> GetCurrentExchangeRateAsync(string baseCurrency)
        {
            using (var ExchangeService = new System.Net.Http.HttpClient())
            {
                using (var res = await ExchangeService.GetAsync(new Uri(string.Format("{0}{1}", "https://api.fixer.io/latest?base=", baseCurrency))))
                {
                    string result = await res.Content.ReadAsStringAsync();

                    return  Newtonsoft.Json.JsonConvert.DeserializeObject<TodayRates>(result);
                }
            }
            
        }
        /// <summary>
        /// Balance
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>

        public async Task<AccountResponse> Balance(int accountNumber)
        {
            return await new Task<AccountResponse>(() =>
              {
                  var response= new AccountResponse();
                  using (var connection =
           new SqlConnection(ConnectionString))
                  {                   

                     var command = new SqlCommand("SP_DEPOSITE_AMOUNT;", connection) { CommandType = System.Data.CommandType.StoredProcedure };
                      var param = new SqlParameter() { Direction = System.Data.ParameterDirection.Output, ParameterName = "@Balance" };
                      command.Parameters.Add(param);
                      command.Parameters.AddWithValue("@accountNumber", accountNumber);
                      try
                      {

                          connection.Open();
                          var result = command.ExecuteNonQuery();
                          if (result > 0)
                          {
                              _balance = Convert.ToDecimal(param.Value);
                              response.Successful = true;
                          }
                          else
                          {
                              response.Successful = false;
                          }
                                                }
                      catch (Exception)
                      {
                          response.Successful = false;
                      }

                  }

                  return response;

              });
        }
        /// <summary>
        /// Deposit
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        public async Task<AccountResponse> Deposit(AccountRequest request)
        {

            var response = new AccountResponse();
            try
            {

                if (request == null)
                {
                    response.Message = string.Format("{0}", "Invalid Request");
                    response.Successful = false;
                    return response;
                }
                response.AccountNumber = request.AccountNumber;

                var account = getAccount(request.AccountNumber);
                if (account == null)
                {
                    response.Message = string.Format("{0}", "sorry! Invalid account credential,Please provide correct account information");
                    response.Successful = false;
                    return response;
                }
                if (account.Amount <=0)
                {
                    response.Message = string.Format("{0}", "Sorry! amount less than the minimum");
                    response.Successful = false;
                    return response;
                }
                var todayRates = GetCurrentExchangeRateAsync(account.Currency).Result;
                var bal = GetAmountWithTodayRate(todayRates.rates, request);
                

                return await new Task<AccountResponse>(() =>
                {

                    using (var connection =
                    new SqlConnection(ConnectionString))
                    {
                        var command = new SqlCommand("SP_DEPOSITE_AMOUNT;", connection) { CommandType = System.Data.CommandType.StoredProcedure };
                        var param = new SqlParameter() { Direction = System.Data.ParameterDirection.Output, ParameterName = "@Balance" };
                        command.Parameters.Add(param);
                        command.Parameters.AddWithValue("@accountNumber", request.AccountNumber);
                        command.Parameters.AddWithValue("@Amount", bal);
                        try
                        {

                            connection.Open();
                            var result = command.ExecuteNonQuery();
                            if (result > 0)
                            {
                                _balance = Convert.ToDecimal(param.Value);
                                response.Successful = true;
                            }
                            else
                            {
                                response.Successful = false;
                            }
                        }
                        catch (Exception)
                        {
                            response.Successful = false;
                        }

                    }
                    if (response.Successful)
                        response.Message = "Transaction is done SuccessFully";
                    else
                        response.Message = "Transaction is done UnSuccessFully";
                    return response;
                });
            }
            catch (Exception)
            {
                response.Successful = false;
                response.Message = "Transaction is done UnSuccessFully";
                return response;
            }
            
        }
        /// <summary>
        /// Withdraw
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<AccountResponse> Withdraw(AccountRequest request)
        {
            var response = new AccountResponse();
            try
            {

                if (request == null)
                {
                    response.Message = string.Format("{0}", "Invalid Request");
                    response.Successful = false;
                    return response;
                }
                response.AccountNumber = request.AccountNumber;

                var account = getAccount(request.AccountNumber);
                if (account == null)
                {
                    response.Message = string.Format("{0}", "sorry! Invalid account credential,Please provide correct account information");
                    response.Successful = false;
                    return response;
                }
                if (account.Amount == -1) {
                    response.Message = string.Format("{0}", "Sorry! Insufficiate fund");
                    response.Successful = false;
                    return response;
                }
                var todayRates = GetCurrentExchangeRateAsync(account.Currency).Result;
                var bal = GetAmountWithTodayRate(todayRates.rates, request);
                if (account.Amount < bal) {
                    response.Message = string.Format("{0}","Sorry! Insufficiate fund");
                    response.Successful = false;
                }
                
                return await new Task<AccountResponse>(() =>
                {

                    using (var connection =
                    new SqlConnection(ConnectionString))
                    {
                        var command = new SqlCommand("SP_WITHDRAW_AMOUNT;", connection) { CommandType = System.Data.CommandType.StoredProcedure };
                        var param = new SqlParameter() { Direction = System.Data.ParameterDirection.Output, ParameterName = "@Balance" };
                        command.Parameters.Add(param);
                        command.Parameters.AddWithValue("@accountNumber", request.AccountNumber);
                        command.Parameters.AddWithValue("@Amount", bal);
                        try
                        {

                            connection.Open();
                            var result = command.ExecuteNonQuery();
                            if (result > 0)
                            {
                                _balance = Convert.ToDecimal(param.Value);
                                response.Successful = true;
                            }
                            else
                            {
                                response.Successful = false;
                            }
                        }
                        catch (Exception)
                        {
                            response.Successful = false;
                        }

                    }
                    if (response.Successful)
                        response.Message = "Transaction is done SuccessFully";
                    else
                        response.Message = "Transaction is done UnSuccessFully";
                    return response;
                });
            }
            catch (Exception)
            {
                response.Successful = false;
                response.Message = "Transaction is done UnSuccessFully";
                return response;
            }
        }
        /// <summary>
        /// GetAmountWithTodayRate
        /// </summary>
        /// <param name="rates"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private decimal GetAmountWithTodayRate(Rates rates, AccountRequest request)
        {
            decimal TotalAmount = -1;
            switch(request.Currency)
            {
                case "AUD":
                    TotalAmount= rates.AUD * request.Amount;
                    break;                
                case "BGN":
                    TotalAmount = rates.BGN * request.Amount;
                    break;
                case "BRL":
                    TotalAmount = rates.BRL * request.Amount;
                    break;
                case "CAD":
                    TotalAmount = rates.CAD * request.Amount;
                    break;
                case "CHF":
                    TotalAmount = rates.CHF * request.Amount;
                    break;
                case "CNY":
                    TotalAmount = rates.CNY * request.Amount;
                    break;
                case "CZK":
                    TotalAmount = rates.CZK * request.Amount;
                    break;
                case "DKK":
                    TotalAmount = rates.DKK * request.Amount;
                    break;
                case "EUR":
                    TotalAmount = rates.EUR * request.Amount;
                    break;
                case "GBP":
                    TotalAmount = rates.GBP * request.Amount;
                    break;
                case "HKD":
                    TotalAmount = rates.HKD* request.Amount;
                    break;
                case "HRK":
                    TotalAmount = rates.HRK * request.Amount;
                    break;
                case "HUF":
                    TotalAmount = rates.HUF * request.Amount;
                    break;
                case "IDR":
                    TotalAmount = rates.IDR * request.Amount;
                    break;
                case "ILS":
                    TotalAmount = rates.ILS * request.Amount;
                    break;
                case "INR":
                    TotalAmount = rates.INR* request.Amount;
                    break;
                case "ISK":
                    TotalAmount = rates.ISK * request.Amount;
                    break;
                case "JPY":
                    TotalAmount = rates.JPY * request.Amount;
                    break;
                case "KRW":
                    TotalAmount = rates.KRW * request.Amount;
                    break;
                case "MXN":
                    TotalAmount = rates.MXN * request.Amount;
                    break;
                case "MYR":
                    TotalAmount = rates.MYR * request.Amount;
                    break;
                case "NOK":
                    TotalAmount = rates.NOK * request.Amount;
                    break;
                case "NZD":
                    TotalAmount = rates.NZD * request.Amount;
                    break;
                case "PHP":
                    TotalAmount = rates.PHP * request.Amount;
                    break;
                case "PLN":
                    TotalAmount = rates.PLN * request.Amount;
                    break;
                case "RON":
                    TotalAmount = rates.RON * request.Amount;
                    break;
                case "RUB":
                    TotalAmount = rates.RUB * request.Amount;
                    break;
                case "SEK":
                    TotalAmount = rates.SEK * request.Amount;
                    break;
                case "SGD":
                    TotalAmount = rates.SGD * request.Amount;
                    break;
                case "TRY":
                    TotalAmount = rates.TRY * request.Amount;
                    break;
                case "USD":
                    TotalAmount = rates.USD * request.Amount;
                    break;
                case "ZAR":
                    TotalAmount = rates.ZAR * request.Amount;
                    break;

            }
            return TotalAmount;
        }
    }
}

using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace RestBankingService.Contract
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IAccountService
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
             UriTemplate = "Balance?accountNumber={accountNumber}",            
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        AccountResponse Balance(int accountNumber);

        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "Deposit",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        AccountResponse Deposit(AccountRequest request);


        [OperationContract]
        [WebInvoke(Method = "POST",
             UriTemplate = "Withdraw",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        AccountResponse Withdraw(AccountRequest request);

        // TODO: Add your service operations here
    }
   
}

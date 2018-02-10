using System.Runtime.Serialization;

namespace RestBankingService.Contract
{
    [DataContract]
    public class AccountRequest
    {
        [DataMember]
        public int AccountNumber { get; set; }
        [DataMember]
        public decimal Amount { get; set; }
        [DataMember]
        public string Currency { get; set; }
    }
}

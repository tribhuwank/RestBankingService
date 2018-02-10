using System.Runtime.Serialization;

namespace RestBankingService.Contract
{
    [DataContract]
    public class AccountResponse
    {
        [DataMember]
        public int AccountNumber { get; set; }
        [DataMember]
        public bool Successful { get; set; }
        [DataMember]
        public decimal Balance { get; set; }
        [DataMember]
        public string Currency { get; set; }
        [DataMember]
        public string Message { get; set; }
    }
}

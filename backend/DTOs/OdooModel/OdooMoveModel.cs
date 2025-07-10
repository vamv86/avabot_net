using Newtonsoft.Json;
using PortaCapena.OdooJsonRpcClient.Attributes;
using PortaCapena.OdooJsonRpcClient.Models;

namespace AiAgentApi.DTOs
{
    [OdooTableName("account.move")]
    public class OdooMoveModel : IOdooModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("payment_state")]
        public string PaymentState { get; set; }

        [JsonProperty("amount_residual")]
        public decimal AmountResidual { get; set; }

        [JsonProperty("amount_total")]
        public decimal AmountTotal { get; set; }
    }

}

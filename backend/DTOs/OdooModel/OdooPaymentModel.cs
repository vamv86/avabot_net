using Newtonsoft.Json;
using PortaCapena.OdooJsonRpcClient.Attributes;
using PortaCapena.OdooJsonRpcClient.Models;

namespace AiAgentApi.DTOs
{
    [OdooTableName("account.payment")]
    public class OdooPaymentModel : IOdooModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("move_id")]
        public long MoveId { get; set; }
    }
}

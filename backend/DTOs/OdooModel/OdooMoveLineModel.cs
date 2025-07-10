using Newtonsoft.Json;
using PortaCapena.OdooJsonRpcClient.Attributes;
using PortaCapena.OdooJsonRpcClient.Models;

namespace AiAgentApi.DTOs
{
    [OdooTableName("account.move.line")]
    public class OdooMoveLineModel : IOdooModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        // Si en algún momento quieres agregar más campos:
        // [JsonProperty("amount_residual")]
        // public decimal AmountResidual { get; set; }
    }

}

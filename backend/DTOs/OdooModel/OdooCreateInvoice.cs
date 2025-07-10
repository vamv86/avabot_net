using Newtonsoft.Json;
using PortaCapena.OdooJsonRpcClient.Attributes;
using PortaCapena.OdooJsonRpcClient.Models;

namespace AiAgentApi.DTOs
{
    [OdooTableName("account.move")]
    public class OdooCreateInvoice : IOdooCreateModel
    {
        [JsonProperty("partner_id")]
        public long ?PartnerId { get; set; }

        [JsonProperty("move_type")]
        public string MoveType { get; set; }

        [JsonProperty("journal_id")]
        public long JournalId { get; set; }

        [JsonProperty("invoice_date")]
        public string InvoiceDate { get; set; }

        [JsonProperty("invoice_line_ids")]
        public object[] InvoiceLineIds { get; set; }
    }
}

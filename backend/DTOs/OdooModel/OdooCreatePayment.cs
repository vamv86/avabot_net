using Newtonsoft.Json;
using PortaCapena.OdooJsonRpcClient.Attributes;
using PortaCapena.OdooJsonRpcClient.Models;

namespace AiAgentApi.DTOs
{
    [OdooTableName("account.payment")]
    public class OdooCreatePayment : IOdooCreateModel
    {
        [JsonProperty("partner_id")]
        public long PartnerId { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("payment_type")]
        public string PaymentType { get; set; } // "inbound" o "outbound"

        [JsonProperty("partner_type")]
        public string PartnerType { get; set; } // "customer" o "supplier"

        [JsonProperty("journal_id")]
        public long JournalId { get; set; }

        [JsonProperty("payment_method_line_id")]
        public long PaymentMethodLineId { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; } // formato "yyyy-MM-dd"

        [JsonProperty("payment_reference")]
        public string PaymentReference { get; set; }

        [JsonProperty("memo")]
        public string Memo { get; set; }

        // 👇 Campo que te faltaba — aquí es donde se asocia el pago con las facturas
        [JsonProperty("invoice_ids")]
        public object[] InvoiceIds { get; set; }
    }
}

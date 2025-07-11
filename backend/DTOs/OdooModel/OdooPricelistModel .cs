using Newtonsoft.Json;
using PortaCapena.OdooJsonRpcClient.Attributes;
using PortaCapena.OdooJsonRpcClient.Models;

namespace AiAgentApi.DTOs
{
    [OdooTableName("product.pricelist")]
    public class OdooPricelistModel : IOdooModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("currency_id")]
        public List<object> CurrencyId { get; set; }  // [id, "USD"]

        [JsonProperty("company_id")]
        public List<object> CompanyId { get; set; }  // [id, "Nombre de la compañía"]

        [JsonProperty("active")]
        public bool Active { get; set; }
    }


}

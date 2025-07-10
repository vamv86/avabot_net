using Newtonsoft.Json;
using PortaCapena.OdooJsonRpcClient.Attributes;
using PortaCapena.OdooJsonRpcClient.Models;

namespace AiAgentApi.DTOs
{
    [OdooTableName("product.product")]
    public class OdooProductModel : IOdooModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("default_code")]
        public string DefaultCode { get; set; }  // Código interno del producto (SKU)

        [JsonProperty("list_price")]
        public decimal ListPrice { get; set; }  // Precio de venta

        [JsonProperty("standard_price")]
        public decimal StandardPrice { get; set; }  // Costo

        [JsonProperty("type")]
        public string Type { get; set; }  // "consu", "service", "product"

        [JsonProperty("active")]
        public bool Active { get; set; }
    }

}

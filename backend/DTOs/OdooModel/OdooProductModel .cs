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
        public decimal ListPrice { get; set; }  // Precio de venta base

        [JsonProperty("currency_id")]
        public List<object> CurrencyId { get; set; }  // [id, "COP"]

        [JsonProperty("standard_price")]
        public decimal StandardPrice { get; set; }  // Costo

        [JsonProperty("type")]
        public string Type { get; set; }  // "consu", "service", "product"

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("product_tmpl_id")]
        public List<object> ProductTemplateId { get; set; }  // [id, "Nombre del template"]
    }

}

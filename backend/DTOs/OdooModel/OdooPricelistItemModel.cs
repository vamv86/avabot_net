using Newtonsoft.Json;
using PortaCapena.OdooJsonRpcClient.Attributes;
using PortaCapena.OdooJsonRpcClient.Models;

namespace AiAgentApi.DTOs
{
    [OdooTableName("product.pricelist.item")]
    public class OdooPricelistItemModel : IOdooModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("pricelist_id")]
        public List<object> PricelistId { get; set; } // [id, "Precio USD"]

        [JsonProperty("product_id")]
        public List<object> ProductId { get; set; } // [id, "Nombre del producto"]

        [JsonProperty("product_tmpl_id")]
        public List<object> ProductTemplateId { get; set; } // [id, "Nombre del template"]

        [JsonProperty("fixed_price")]
        public decimal FixedPrice { get; set; }

        [JsonProperty("currency_id")]
        public List<object> CurrencyId { get; set; } // [id, "USD"]

        [JsonProperty("min_quantity")]
        public decimal MinQuantity { get; set; }
    }

}

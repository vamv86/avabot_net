using Newtonsoft.Json;
using PortaCapena.OdooJsonRpcClient.Attributes;
using PortaCapena.OdooJsonRpcClient.Models;

namespace AiAgentApi.DTOs
{
    [OdooTableName("res.currency")]
    public class OdooCurrencyModel : IOdooModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty; // Ej: "USD", "COP"

        [JsonProperty("symbol")]
        public string Symbol { get; set; } = string.Empty; // Ej: "$"

        [JsonProperty("rate")]
        public decimal Rate { get; set; } // Tasa de conversión

        [JsonProperty("rounding")]
        public decimal Rounding { get; set; }

        [JsonProperty("position")]
        public string Position { get; set; } = string.Empty; // "before" o "after"

        [JsonProperty("decimal_places")]
        public int DecimalPlaces { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; } = true;
    }

}

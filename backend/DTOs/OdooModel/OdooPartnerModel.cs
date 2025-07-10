using Newtonsoft.Json;
using PortaCapena.OdooJsonRpcClient.Attributes;
using PortaCapena.OdooJsonRpcClient.Models;
using System.Text.Json.Serialization;

namespace AiAgentApi.DTOs
{
    [OdooTableName("res.partner")]
    public class OdooPartnerModel : IOdooModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("vat")]
        public string Vat { get; set; } // NIT o documento de identidad

        [JsonProperty("company_type")]
        public string CompanyType { get; set; } // "person" o "company"

        [JsonProperty("active")]
        public bool Active { get; set; }
    }

    [OdooTableName("res.partner")]
    public class OdooCreatePartnerModel : IOdooCreateModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("company_type")]
        public string CompanyType { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; } = true;
    }

}

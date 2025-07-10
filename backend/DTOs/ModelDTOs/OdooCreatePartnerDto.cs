using Newtonsoft.Json;
using PortaCapena.OdooJsonRpcClient.Attributes;
using PortaCapena.OdooJsonRpcClient.Models;

namespace AiAgentApi.DTOs.ModelDTOs
{
    public class OdooCreatePartnerDto
    {
        public string Name { get; set; }           // Nombre del contacto
        public string Email { get; set; }          // Correo electrónico
        public string Phone { get; set; }          // Teléfono
        public string CompanyType { get; set; }    // "person" o "company"
        public string Street { get; set; }         // Dirección
        public string City { get; set; }           // Ciudad
    }

}

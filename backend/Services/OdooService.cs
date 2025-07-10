
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using AiAgentApi.DTOs;
using Microsoft.Extensions.Configuration;
using PortaCapena.OdooJsonRpcClient.Models;
using PortaCapena.OdooJsonRpcClient.Request;
using PortaCapena.OdooJsonRpcClient;
using AiAgentApi.DTOs.ModelDTOs;
using Microsoft.EntityFrameworkCore;
using AiAgentApi.Data;
namespace AiAgentApi.Services;

public interface IOdooService
{
    Task<string> ProcesarPagoBoldAsync(BoldConfirmationDto dto);
    Task<(bool Success, string Message, long? PartnerId)> CrearPartnerAsync(OdooCreatePartnerDto dto);
    Task<OdooProductModel> ObtenerProductoPOrNombreAsync(string name);

}

public class OdooService : IOdooService
{
    private readonly OdooClient _odooClient;

    private readonly ApplicationDbContext _context;
 
    public OdooService(ApplicationDbContext context)
    {
        _context = context;

        var config = new OdooConfig(
            apiUrl: "https://avainnova01-odoo-prod-17875069.dev.odoo.com",
            dbName: "avainnova01-odoo-prod-17875069",
            userName: "andersonscoin@gmail.com",
            password: "Ander28."
        );

        _odooClient = new OdooClient(config);
    }

    /// <summary>
    /// Author: Victor Moreno  
    /// Descripción: Procesa la confirmación de pago proveniente de Bold creando una factura, validándola,
    /// registrando el pago y conciliando en Odoo. Si el contacto no existe, lo crea automáticamente.
    /// Fecha: 2025-07-09
    /// </summary>
    /// <param name="dto">DTO con la información enviada por Bold.</param>
    /// <returns>Mensaje con el resultado del proceso.</returns>
    public async Task<string> ProcesarPagoBoldAsync(BoldConfirmationDto dto)
    {
        var uid = (await _odooClient.LoginAsync()).Value;

        var journalId = (await _odooClient.GetAsync<OdooJournalModel>(new OdooQuery
        {  
            Filters = new OdooFilter().EqualTo("name", "Customer Invoices"),
            ReturnFields = { "id" }
        })).Value.FirstOrDefault()?.Id ?? throw new Exception("Journal no encontrado");

        // Validar si el contacto existe por email
        var partnerId = (await _odooClient.GetAsync<OdooPartnerModel>(new OdooQuery
        {
            Filters = new OdooFilter().EqualTo("email", dto.Data.PayerEmail),
            ReturnFields = { "id" }
        })).Value.FirstOrDefault()?.Id;


        if (partnerId == null)
        {
            var user = await _context.Users
               .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Data.PayerEmail.ToLower());
            string phone = string.Empty;
            if (user != null)
            {
                phone = user.WhatsApp;
            }

                var nuevoContacto = new OdooCreatePartnerDto
            {
                Name = dto.Data.PayerEmail.Split('@')[0],
                Email = dto.Data.PayerEmail,
                Phone = phone,
                CompanyType = "person",
                Street = string.Empty,
                City = string.Empty
            };

            var creationResult = await CrearPartnerAsync(nuevoContacto);
            if (!creationResult.Success)
            {
                throw new Exception("No se pudo crear el contacto automáticamente: " + creationResult.Message);
            }

            partnerId = creationResult.PartnerId.Value;
        }

        var productId = (await _odooClient.GetAsync<OdooProductModel>(new OdooQuery
        {
            Filters = new OdooFilter().EqualTo("name", "AvaBot"),
            ReturnFields = { "id" }
        })).Value.FirstOrDefault()?.Id ?? throw new Exception("Producto no encontrado");

        var unitPrice = dto.Data.Amount.Total;

        var invoiceModel = new OdooCreateInvoice
        {
            PartnerId = partnerId,
            MoveType = "out_invoice",
            JournalId = journalId,
            InvoiceDate = DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            InvoiceLineIds = new object[]
            {
            new object[]
            {
                0, 0, new Dictionary<string, object>
                {
                    { "product_id", productId },
                    { "quantity", 1 },
                    { "price_unit", unitPrice },
                    { "name", "Pago Bold - " + dto.Data.Metadata?.Reference }
                }
            }
            }
        };

        var invoiceId = (await _odooClient.CreateAsync(invoiceModel)).Value;

        await OdooClient.CallAndDeserializeAsync<bool>(
            OdooAvaRequestModelHelper.ExecuteMethod(
                _odooClient.Config, uid, "account.move", "action_post", new object[] { invoiceId }));

        var bankJournalId = (await _odooClient.GetAsync<OdooJournalModel>(new OdooQuery
        {
            Filters = new OdooFilter().EqualTo("name", "Bank"),
            ReturnFields = { "id" }
        })).Value.FirstOrDefault()?.Id ?? throw new Exception("Bank Journal no encontrado");

        var methodId = (await _odooClient.GetAsync<OdooPaymentMethodLineModel>(new OdooQuery
        {
            Filters = new OdooFilter().EqualTo("name", "Manual Payment"),
            ReturnFields = { "id" }
        })).Value.FirstOrDefault()?.Id ?? throw new Exception("Método de pago no encontrado");

        var paymentModel = new OdooCreatePayment
        {
            PartnerId = partnerId.Value,
            Amount = unitPrice,
            PaymentType = "inbound",
            PartnerType = "customer",
            JournalId = bankJournalId,
            PaymentMethodLineId = methodId,
            Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            PaymentReference = $"Pago desde Bold {dto.Data.PaymentId}",
            Memo = $"Pago Bold - {dto.Data.Metadata?.Reference}",
            InvoiceIds = new object[]
            {
            new object[] { 4, invoiceId }
            }
        };

        var paymentId = (await _odooClient.CreateAsync(paymentModel)).Value;

        await OdooClient.CallAndDeserializeAsync<bool>(
            OdooAvaRequestModelHelper.ExecuteMethod(
                _odooClient.Config, uid, "account.payment", "action_post", new object[] { paymentId }));

        return $"✅ Factura {invoiceId} y pago {paymentId} procesados.";
    }


    /// <summary>
    /// Author: Victor Moreno  
    /// Descripción: Crea un nuevo contacto (partner) en Odoo si no existe uno con el mismo correo electrónico.  
    /// Fecha: 2025-07-09
    /// </summary>
    /// <param name="dto">Datos del contacto a crear.</param>
    /// <returns>Tupla indicando si fue exitoso, mensaje de estado y ID del partner creado (si aplica).</returns>
    public async Task<(bool Success, string Message, long? PartnerId)> CrearPartnerAsync(OdooCreatePartnerDto dto)
    {
        var uid = (await _odooClient.LoginAsync()).Value;

        var checkQuery = new OdooQuery();
        checkQuery.Filters = new OdooFilter().EqualTo("email", dto.Email);
        checkQuery.ReturnFields.Add("id");

        var checkResult = await _odooClient.GetAsync<OdooPartnerModel>(checkQuery);
        if (checkResult.Value.Any())
        {
            return (false, $"El contacto con correo '{dto.Email}' ya existe.", null);
        }

        var newPartner = new OdooCreatePartnerModel
        {
            Name = dto.Name,
            Email = dto.Email,   
            Phone = dto.Phone,
            CompanyType = dto.CompanyType,
            Street = dto.Street,
            City = dto.City
        };

        var partnerResponse = await _odooClient.CreateAsync(newPartner);
        if (!partnerResponse.Succeed)
        {
            return (false, $"Error al crear el contacto: {partnerResponse.Error.Message}", null);
        }

        return (true, "Contacto creado exitosamente", partnerResponse.Value);
    }


    /// <summary>
    /// Author: Victor Moreno  
    /// Descripción: Consulta un producto en Odoo por su nombre exacto y retorna su información básica,
    /// incluyendo ID, nombre, precio estándar, tipo de producto, unidad de medida, etc.
    /// Lanza una excepción si el producto no se encuentra.
    /// Fecha: 2025-07-09
    /// </summary>
    /// <param name="name">Nombre exacto del producto en Odoo.</param>
    /// <returns>Modelo <see cref="OdooProductModel"/> del producto encontrado con detalles clave.</returns>
    /// <exception cref="Exception">Si no se encuentra el producto con el nombre especificado.</exception>
    public async Task<OdooProductModel> ObtenerProductoPOrNombreAsync(string name)
    {
        var producto = (await _odooClient.GetAsync<OdooProductModel>(new OdooQuery
        {
            Filters = new OdooFilter().EqualTo("name", name),
            ReturnFields = {
            "id",
            "name",
            "list_price",
            "type",
            "uom_id",
            "currency_id",
            "default_code",
            "sale_ok",
            "purchase_ok"
        }
        })).Value.FirstOrDefault();

        if (producto == null)
            throw new Exception($"Producto '{name}' no encontrado.");

        return producto;
    }

}

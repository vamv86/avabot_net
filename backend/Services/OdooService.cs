
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
using AiAgentApi.Models;
namespace AiAgentApi.Services;

public interface IOdooService
{
    Task<string> ProcesarPagoBoldAsync(BoldConfirmationDto dto);
    Task<(bool Success, string Message, long? PartnerId)> CrearPartnerAsync(OdooCreatePartnerDto dto);
    Task<OdooProductModel> ObtenerProductoPorNombreAsync(string name);

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
    //public async Task<string> ProcesarPagoBoldAsync(BoldConfirmationDto dto)
    //{
    //    var uid = (await _odooClient.LoginAsync()).Value;

    //    var journalId = (await _odooClient.GetAsync<OdooJournalModel>(new OdooQuery
    //    {  
    //        Filters = new OdooFilter().EqualTo("name", "Customer Invoices"),
    //        ReturnFields = { "id" }
    //    })).Value.FirstOrDefault()?.Id ?? throw new Exception("Journal no encontrado");

    //    // Validar si el contacto existe por email
    //    var partnerId = (await _odooClient.GetAsync<OdooPartnerModel>(new OdooQuery
    //    {
    //        Filters = new OdooFilter().EqualTo("email", dto.Data.PayerEmail),
    //        ReturnFields = { "id" }
    //    })).Value.FirstOrDefault()?.Id;


    //    if (partnerId == null)
    //    {
    //        var user = await _context.Users
    //           .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Data.PayerEmail.ToLower());
    //        string phone = string.Empty;
    //        if (user != null)
    //        {
    //            phone = user.WhatsApp;
    //        }

    //            var nuevoContacto = new OdooCreatePartnerDto
    //        {
    //            Name = dto.Data.PayerEmail.Split('@')[0],
    //            Email = dto.Data.PayerEmail,
    //            Phone = phone,
    //            CompanyType = "person",
    //            Street = string.Empty,
    //            City = string.Empty
    //        };

    //        var creationResult = await CrearPartnerAsync(nuevoContacto);
    //        if (!creationResult.Success)
    //        {
    //            throw new Exception("No se pudo crear el contacto automáticamente: " + creationResult.Message);
    //        }

    //        partnerId = creationResult.PartnerId.Value;
    //    }

    //    var productId = (await _odooClient.GetAsync<OdooProductModel>(new OdooQuery
    //    {
    //        Filters = new OdooFilter().EqualTo("name", "AvaBot"),
    //        ReturnFields = { "id" }
    //    })).Value.FirstOrDefault()?.Id ?? throw new Exception("Producto no encontrado");

    //    var unitPrice = dto.Data.Amount.Total;

    //    var invoiceModel = new OdooCreateInvoice
    //    {
    //        PartnerId = partnerId,
    //        MoveType = "out_invoice",
    //        JournalId = journalId,
    //        InvoiceDate = DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
    //        InvoiceLineIds = new object[]
    //        {
    //        new object[]
    //        {
    //            0, 0, new Dictionary<string, object>
    //            {
    //                { "product_id", productId },
    //                { "quantity", 1 },
    //                { "price_unit", unitPrice },
    //                { "name", "Pago Bold - " + dto.Data.Metadata?.Reference }
    //            }
    //        }
    //        }
    //    };

    //    var invoiceId = (await _odooClient.CreateAsync(invoiceModel)).Value;

    //    await OdooClient.CallAndDeserializeAsync<bool>(
    //        OdooAvaRequestModelHelper.ExecuteMethod(
    //            _odooClient.Config, uid, "account.move", "action_post", new object[] { invoiceId }));

    //    var bankJournalId = (await _odooClient.GetAsync<OdooJournalModel>(new OdooQuery
    //    {
    //        Filters = new OdooFilter().EqualTo("name", "Bank"),
    //        ReturnFields = { "id" }
    //    })).Value.FirstOrDefault()?.Id ?? throw new Exception("Bank Journal no encontrado");

    //    var methodId = (await _odooClient.GetAsync<OdooPaymentMethodLineModel>(new OdooQuery
    //    {
    //        Filters = new OdooFilter().EqualTo("name", "Manual Payment"),
    //        ReturnFields = { "id" }
    //    })).Value.FirstOrDefault()?.Id ?? throw new Exception("Método de pago no encontrado");

    //    var paymentModel = new OdooCreatePayment
    //    {
    //        PartnerId = partnerId.Value,
    //        Amount = unitPrice,
    //        PaymentType = "inbound",
    //        PartnerType = "customer",
    //        JournalId = bankJournalId,
    //        PaymentMethodLineId = methodId,
    //        Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
    //        PaymentReference = $"Pago desde Bold {dto.Data.PaymentId}",
    //        Memo = $"Pago Bold - {dto.Data.Metadata?.Reference}",
    //        InvoiceIds = new object[]
    //        {
    //        new object[] { 4, invoiceId }
    //        }
    //    };

    //    var paymentId = (await _odooClient.CreateAsync(paymentModel)).Value;

    //    await OdooClient.CallAndDeserializeAsync<bool>(
    //        OdooAvaRequestModelHelper.ExecuteMethod(
    //            _odooClient.Config, uid, "account.payment", "action_post", new object[] { paymentId }));

    //    return $"✅ Factura {invoiceId} y pago {paymentId} procesados.";
    //}


    //public async Task<string> ProcesarPagoBoldAsync(BoldConfirmationDto dto)
    //{
    //    var uid = (await _odooClient.LoginAsync()).Value;

    //    if (string.IsNullOrWhiteSpace(dto.Data.Amount.Currency))
    //        throw new Exception("❌ El campo Currency no puede estar vacío.");

    //    // 1. Obtener producto de Odoo
    //    var product = (await _odooClient.GetAsync<OdooProductModel>(new OdooQuery
    //    {
    //        Filters = new OdooFilter().EqualTo("name", "AvaBot"),
    //        ReturnFields = { "id", "name", "list_price", "default_code" }
    //    })).Value.FirstOrDefault() ?? throw new Exception("Producto no encontrado");

    //    var productId = product.Id;

    //    // 2. Journal de facturación
    //    var journalId = (await _odooClient.GetAsync<OdooJournalModel>(new OdooQuery
    //    {
    //        Filters = new OdooFilter().EqualTo("name", "Customer Invoices"),
    //        ReturnFields = { "id" }
    //    })).Value.FirstOrDefault()?.Id ?? throw new Exception("Journal no encontrado");

    //    // 3. Obtener o crear usuario local
    //    var userLocal = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Data.PayerEmail.ToLower());
    //    if (userLocal == null)
    //    {
    //        userLocal = new User
    //        {
    //            Email = dto.Data.PayerEmail,
    //            Name = dto.Data.PayerEmail.Split('@')[0],
    //            WhatsApp = dto.Data.PayerPhone ?? string.Empty,
    //            CreatedAt = DateTime.UtcNow,
    //            UpdatedAt = DateTime.UtcNow
    //        };

    //        _context.Users.Add(userLocal);
    //        await _context.SaveChangesAsync();
    //    }

    //    // 4. Obtener o crear partner en Odoo
    //    var partnerId = (await _odooClient.GetAsync<OdooPartnerModel>(new OdooQuery
    //    {
    //        Filters = new OdooFilter().EqualTo("email", userLocal.Email),
    //        ReturnFields = { "id" }
    //    })).Value.FirstOrDefault()?.Id;

    //    if (partnerId == null)
    //    {
    //        var nuevoContacto = new OdooCreatePartnerDto
    //        {
    //            Name = userLocal.Name,
    //            Email = userLocal.Email,
    //            Phone = userLocal.WhatsApp ?? string.Empty,
    //            CompanyType = "person"
    //        };

    //        var creationResult = await CrearPartnerAsync(nuevoContacto);
    //        if (!creationResult.Success)
    //            throw new Exception("No se pudo crear el contacto automáticamente: " + creationResult.Message);

    //        partnerId = creationResult.PartnerId.Value;
    //    }

    //    // 5. Crear factura
    //    var invoiceModel = new OdooCreateInvoice
    //    {
    //        PartnerId = partnerId,
    //        MoveType = "out_invoice",
    //        JournalId = journalId,
    //        InvoiceDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
    //        InvoiceLineIds = new object[]
    //        {
    //        new object[]
    //        {
    //            0, 0, new Dictionary<string, object>
    //            {
    //                { "product_id", productId },
    //                { "quantity", 1 },
    //                { "price_unit", dto.Data.Amount.Total },
    //                { "name", "Pago Bold - " + dto.Data.Metadata?.Reference }
    //            }
    //        }
    //        }
    //    };

    //    var invoiceId = (await _odooClient.CreateAsync(invoiceModel)).Value;

    //    await OdooClient.CallAndDeserializeAsync<bool>(
    //        OdooAvaRequestModelHelper.ExecuteMethod(_odooClient.Config, uid, "account.move", "action_post", new object[] { invoiceId }));

    //    // 6. Crear y conciliar el pago
    //    var bankJournalId = (await _odooClient.GetAsync<OdooJournalModel>(new OdooQuery
    //    {
    //        Filters = new OdooFilter().EqualTo("name", "Bank"),
    //        ReturnFields = { "id" }
    //    })).Value.FirstOrDefault()?.Id ?? throw new Exception("Bank Journal no encontrado");

    //    var methodId = (await _odooClient.GetAsync<OdooPaymentMethodLineModel>(new OdooQuery
    //    {
    //        Filters = new OdooFilter().EqualTo("name", "Manual Payment"),
    //        ReturnFields = { "id" }
    //    })).Value.FirstOrDefault()?.Id ?? throw new Exception("Método de pago no encontrado");

    //    var paymentModel = new OdooCreatePayment
    //    {
    //        PartnerId = partnerId.Value,
    //        Amount = dto.Data.Amount.Total,
    //        PaymentType = "inbound",
    //        PartnerType = "customer",
    //        JournalId = bankJournalId,
    //        PaymentMethodLineId = methodId,
    //        Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
    //        PaymentReference = $"Pago desde Bold {dto.Data.PaymentId}",
    //        Memo = $"Pago Bold - {dto.Data.Metadata?.Reference}",
    //        InvoiceIds = new object[] { new object[] { 4, invoiceId } }  // Relación directa
    //    };

    //    var paymentId = (await _odooClient.CreateAsync(paymentModel)).Value;

    //    await OdooClient.CallAndDeserializeAsync<bool>(
    //        OdooAvaRequestModelHelper.ExecuteMethod(_odooClient.Config, uid, "account.payment", "action_post", new object[] { paymentId }));

    //    // 7. Crear suscripción local si no existe
    //    var subscription = await _context.Subscriptions.FirstOrDefaultAsync(s =>
    //        s.UserId == userLocal.Id &&
    //        s.ExternalProductId == productId.ToString() &&
    //        s.Status == "active");

    //    if (subscription == null)
    //    {
    //        subscription = new Subscription
    //        {
    //            UserId = userLocal.Id,
    //            ExternalProductId = productId.ToString(),
    //            ProductName = product.Name,
    //            PlanName = product.Name,
    //            Amount = (decimal)dto.Data.Amount.Total,
    //            Currency = dto.Data.Amount.Currency,
    //            Status = "active",
    //            StartDate = DateTime.UtcNow,
    //            CreatedAt = DateTime.UtcNow,
    //            UpdatedAt = DateTime.UtcNow
    //        };

    //        _context.Subscriptions.Add(subscription);
    //        await _context.SaveChangesAsync();
    //    }

    //    // 8. Crear registro de pago local
    //    var payment = new Payment
    //    {
    //        SubscriptionId = subscription.Id,
    //        PaymentId = dto.Data.PaymentId,
    //        TransactionId = dto.Data.Metadata?.Reference,
    //        Amount = dto.Data.Amount.Total,
    //        Currency = dto.Data.Amount.Currency,
    //        Status = "completed",
    //        PaymentProvider = "Bold",
    //        PaymentType = "subscription",
    //        PaymentDate = DateTime.SpecifyKind(dto.Data.CreatedAt, DateTimeKind.Utc),
    //        CreatedAt = DateTime.UtcNow,
    //        UpdatedAt = DateTime.UtcNow
    //    };

    //    _context.Payments.Add(payment);
    //    await _context.SaveChangesAsync();

    //    return $"✅ Factura {invoiceId} y pago {paymentId} procesados y conciliados correctamente.";
    //}


    //public async Task<string> ProcesarPagoBoldAsync(BoldConfirmationDto dto)
    //{
    //    var uid = (await _odooClient.LoginAsync()).Value;

    //    if (string.IsNullOrWhiteSpace(dto.Data.Amount.Currency))
    //        throw new Exception("❌ El campo Currency no puede estar vacío.");

    //    // Redondear monto una vez para usarlo en toda la operación
    //    var monto = Math.Round(dto.Data.Amount.Total, 2, MidpointRounding.AwayFromZero);

    //    // 1. Obtener producto de Odoo
    //    var product = (await _odooClient.GetAsync<OdooProductModel>(new OdooQuery
    //    {
    //        Filters = new OdooFilter().EqualTo("name", "AvaBot"),
    //        ReturnFields = { "id", "name", "list_price", "default_code" }
    //    })).Value.FirstOrDefault() ?? throw new Exception("Producto no encontrado");

    //    var productId = product.Id;

    //    // 2. Journal de facturación
    //    var journalId = (await _odooClient.GetAsync<OdooJournalModel>(new OdooQuery
    //    {
    //        Filters = new OdooFilter().EqualTo("name", "Customer Invoices"),
    //        ReturnFields = { "id" }
    //    })).Value.FirstOrDefault()?.Id ?? throw new Exception("Journal no encontrado");

    //    // 3. Obtener o crear usuario local
    //    var userLocal = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Data.PayerEmail.ToLower());
    //    if (userLocal == null)
    //    {
    //        userLocal = new User
    //        {
    //            Email = dto.Data.PayerEmail,
    //            Name = dto.Data.PayerEmail.Split('@')[0],
    //            WhatsApp = dto.Data.PayerPhone ?? string.Empty,
    //            CreatedAt = DateTime.UtcNow,
    //            UpdatedAt = DateTime.UtcNow
    //        };

    //        _context.Users.Add(userLocal);
    //        await _context.SaveChangesAsync();
    //    }

    //    // 4. Obtener o crear partner en Odoo
    //    var partnerId = (await _odooClient.GetAsync<OdooPartnerModel>(new OdooQuery
    //    {
    //        Filters = new OdooFilter().EqualTo("email", userLocal.Email),
    //        ReturnFields = { "id" }
    //    })).Value.FirstOrDefault()?.Id;

    //    if (partnerId == null)
    //    {
    //        var nuevoContacto = new OdooCreatePartnerDto
    //        {
    //            Name = userLocal.Name,
    //            Email = userLocal.Email,
    //            Phone = userLocal.WhatsApp ?? string.Empty,
    //            CompanyType = "person"
    //        };

    //        var creationResult = await CrearPartnerAsync(nuevoContacto);
    //        if (!creationResult.Success)
    //            throw new Exception("No se pudo crear el contacto automáticamente: " + creationResult.Message);

    //        partnerId = creationResult.PartnerId.Value;
    //    }

    //    // 5. Crear factura
    //    var invoiceModel = new OdooCreateInvoice
    //    {
    //        PartnerId = partnerId,
    //        MoveType = "out_invoice",
    //        JournalId = journalId,
    //        InvoiceDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
    //        InvoiceLineIds = new object[]
    //        {
    //        new object[]
    //        {
    //            0, 0, new Dictionary<string, object>
    //            {
    //                { "product_id", productId },
    //                { "quantity", 1 },
    //                { "price_unit", monto },
    //                { "name", "Pago Bold - " + dto.Data.Metadata?.Reference }
    //            }
    //        }
    //        }
    //    };

    //    var invoiceId = (await _odooClient.CreateAsync(invoiceModel)).Value;

    //    await OdooClient.CallAndDeserializeAsync<bool>(
    //        OdooAvaRequestModelHelper.ExecuteMethod(_odooClient.Config, uid, "account.move", "action_post", new object[] { invoiceId }));

    //    // 6. Crear y conciliar el pago
    //    var bankJournalId = (await _odooClient.GetAsync<OdooJournalModel>(new OdooQuery
    //    {
    //        Filters = new OdooFilter().EqualTo("name", "Bank"),
    //        ReturnFields = { "id" }
    //    })).Value.FirstOrDefault()?.Id ?? throw new Exception("Bank Journal no encontrado");

    //    var methodId = (await _odooClient.GetAsync<OdooPaymentMethodLineModel>(new OdooQuery
    //    {
    //        Filters = new OdooFilter().EqualTo("name", "Manual Payment"),
    //        ReturnFields = { "id" }
    //    })).Value.FirstOrDefault()?.Id ?? throw new Exception("Método de pago no encontrado");

    //    var paymentModel = new OdooCreatePayment
    //    {
    //        PartnerId = partnerId.Value,
    //        Amount = monto,
    //        PaymentType = "inbound",
    //        PartnerType = "customer",
    //        JournalId = bankJournalId,
    //        PaymentMethodLineId = methodId,
    //        Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
    //        PaymentReference = $"Pago desde Bold {dto.Data.PaymentId}",
    //        Memo = $"Pago Bold - {dto.Data.Metadata?.Reference}",
    //        InvoiceIds = new object[] { new object[] { 4, invoiceId } }           
    //    };

    //    var paymentId = (await _odooClient.CreateAsync(paymentModel)).Value;

    //    await OdooClient.CallAndDeserializeAsync<bool>(
    //        OdooAvaRequestModelHelper.ExecuteMethod(_odooClient.Config, uid, "account.payment", "action_post", new object[] { paymentId }));

    //    // 7. Crear suscripción local si no existe
    //    var subscription = await _context.Subscriptions.FirstOrDefaultAsync(s =>
    //        s.UserId == userLocal.Id &&
    //        s.ExternalProductId == productId.ToString() &&
    //        s.Status == "active");

    //    if (subscription == null)
    //    {
    //        subscription = new Subscription
    //        {
    //            UserId = userLocal.Id,
    //            ExternalProductId = productId.ToString(),
    //            ProductName = product.Name,
    //            PlanName = product.Name,
    //            Amount = monto,
    //            Currency = dto.Data.Amount.Currency,
    //            Status = "active",
    //            StartDate = DateTime.UtcNow,
    //            CreatedAt = DateTime.UtcNow,
    //            UpdatedAt = DateTime.UtcNow
    //        };

    //        _context.Subscriptions.Add(subscription);
    //        await _context.SaveChangesAsync();
    //    }

    //    // 8. Crear registro de pago local
    //    var payment = new Payment
    //    {
    //        SubscriptionId = subscription.Id,
    //        PaymentId = dto.Data.PaymentId,
    //        TransactionId = dto.Data.Metadata?.Reference,
    //        Amount = monto,
    //        Currency = dto.Data.Amount.Currency,
    //        Status = "completed",
    //        PaymentProvider = "Bold",
    //        PaymentType = "subscription",
    //        PaymentDate = DateTime.SpecifyKind(dto.Data.CreatedAt, DateTimeKind.Utc),
    //        CreatedAt = DateTime.UtcNow,
    //        UpdatedAt = DateTime.UtcNow
    //    };

    //    _context.Payments.Add(payment);
    //    await _context.SaveChangesAsync();

    //    return $"✅ Factura {invoiceId} y pago {paymentId} procesados y conciliados correctamente.";
    //}


    public async Task<string> ProcesarPagoBoldAsync(BoldConfirmationDto dto)
    {
        var uid = (await _odooClient.LoginAsync()).Value;

        // Validar que Currency no sea null
        if (string.IsNullOrWhiteSpace(dto.Data.Amount.Currency))
            throw new Exception("❌ El campo Currency no puede estar vacío.");

        // Obtener el producto desde Odoo
       var product = await ObtenerProductoPorNombreAsync("AvaBot");


       var productId = product.Id;

        var journalId = (await _odooClient.GetAsync<OdooJournalModel>(new OdooQuery
        {
            Filters = new OdooFilter().EqualTo("name", "Customer Invoices"),
            ReturnFields = { "id" }
        })).Value.FirstOrDefault()?.Id ?? throw new Exception("Journal no encontrado");

        // Asegurar que el usuario exista en BD
        var userLocal = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Data.PayerEmail.ToLower());

        if (userLocal == null)
        {
            userLocal = new User
            {
                Email = dto.Data.PayerEmail,
                Name = dto.Data.PayerEmail.Split('@')[0],
                WhatsApp = dto.Data.PayerPhone ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(userLocal);
            await _context.SaveChangesAsync();
        }

        // Buscar o crear el contacto en Odoo
        var partnerId = (await _odooClient.GetAsync<OdooPartnerModel>(new OdooQuery
        {
            Filters = new OdooFilter().EqualTo("email", userLocal.Email),
            ReturnFields = { "id" }
        })).Value.FirstOrDefault()?.Id;

        if (partnerId == null)
        {
            var nuevoContacto = new OdooCreatePartnerDto
            {
                Name = userLocal.Name,
                Email = userLocal.Email,
                Phone = userLocal.WhatsApp ?? string.Empty,
                CompanyType = "person",
                Street = string.Empty,
                City = string.Empty
            };

            var creationResult = await CrearPartnerAsync(nuevoContacto);
            if (!creationResult.Success)
                throw new Exception("No se pudo crear el contacto automáticamente: " + creationResult.Message);

            partnerId = creationResult.PartnerId.Value;
        }


        var currencyId = (await _odooClient.GetAsync<OdooCurrencyModel>(new OdooQuery
        {
            Filters = new OdooFilter().EqualTo("name", dto.Data.Amount.Currency),
            ReturnFields = { "id" }
        })).Value.FirstOrDefault()?.Id ?? throw new Exception("❌ Moneda no encontrada en Odoo");

        // Crear factura en Odoo
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
                        { "price_unit", dto.Data.Amount.Total },
                        { "name", "Pago Bold - " + dto.Data.Metadata?.Reference }
                    }
                }
            },
            CurrencyId = currencyId
        };

        var invoiceId = (await _odooClient.CreateAsync(invoiceModel)).Value;

        await OdooClient.CallAndDeserializeAsync<bool>(
            OdooAvaRequestModelHelper.ExecuteMethod(
                _odooClient.Config, uid, "account.move", "action_post", new object[] { invoiceId }));

        // Crear pago en Odoo
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
            Amount = dto.Data.Amount.Total,
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
            },
            CurrencyId = currencyId
        };

        var paymentId = (await _odooClient.CreateAsync(paymentModel)).Value;

        await OdooClient.CallAndDeserializeAsync<bool>(
            OdooAvaRequestModelHelper.ExecuteMethod(
                _odooClient.Config, uid, "account.payment", "action_post", new object[] { paymentId }));

        // Crear suscripción local si no existe
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == userLocal.Id &&
                                      s.ExternalProductId == productId.ToString() &&
                                      s.Status == "active");

        if (subscription == null)
        {
            subscription = new Subscription
            {
                UserId = userLocal.Id,
                ExternalProductId = productId.ToString(),
                ProductName = product.Name,
                PlanName = product.Name,
                Amount = (decimal)dto.Data.Amount.Total,
                Currency = dto.Data.Amount.Currency,
                Status = "active",
                StartDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();
        }


        // Crear método de pago local si no existe
        var metodoExistente = await _context.PaymentMethods
            .FirstOrDefaultAsync(m => m.ExternalId == dto.Data.PaymentId); // o algún otro identificador

        if (metodoExistente == null)
        {
            metodoExistente = new PaymentMethod
            {
                Provider = "Bold",
                ExternalId = dto.Data.PaymentId,
                Token = string.Empty,
                Brand = dto.Data.PaymentMethod,
                Last4 = dto.Data.PayerEmail[^4..], // ejemplo, ajustar según sea real
                Type = "card", // ajustar según sea real
                UserId = userLocal.Id,
                CreatedAt = DateTime.UtcNow
            };

            _context.PaymentMethods.Add(metodoExistente);
            await _context.SaveChangesAsync();
        }


        // Crear registro de pago local
        var payment = new Payment
        {
            SubscriptionId = subscription.Id,
            PaymentId = dto.Data.PaymentId,
            TransactionId = dto.Data.Metadata?.Reference,
            Amount = dto.Data.Amount.Total,
            Currency = dto.Data.Amount.Currency,
            Status = "completed",
            PaymentProvider = "Bold",
            PaymentType = "subscription",
            PaymentMethod = metodoExistente,
            PaymentDate = DateTime.SpecifyKind(dto.Data.CreatedAt, DateTimeKind.Utc),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        return $"✅ Factura {invoiceId} y pago {paymentId} procesados correctamente.";
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
    public async Task<OdooProductModel> ObtenerProductoPorNombreAsync(string name)
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
        "purchase_ok",
        "product_tmpl_id"
    }
        })).Value.FirstOrDefault();

        if (producto == null)
            throw new Exception($"Producto '{name}' no encontrado.");


        try
        {
            // se saca la lista de precios del producto
            var precios = await _odooClient.GetAsync<OdooPricelistItemModel>(new OdooQuery
            {
                Filters = new OdooFilter()
                    .EqualTo("product_tmpl_id", producto.ProductTemplateId[0]), // usas el template ID
                ReturnFields = { "fixed_price", "pricelist_id", "currency_id" }
            });

            var currency = precios.Value[0].CurrencyId;
            decimal price = precios.Value[0].FixedPrice;

            producto.CurrencyId = currency;
            producto.ListPrice = price;

        }
        catch (Exception ex)
        {

        }



        if (producto == null)
            throw new Exception($"Producto '{name}' no encontrado.");

        return producto;
    }

}

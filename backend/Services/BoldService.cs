
using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
namespace AiAgentApi.Services;

public interface IBoldService
{
    string GenerateSignature(string orderId, string amount, string currency);
    string GetSuccessUrl();
    string GetFailureUrl();
    string GetBaseUrl();
}


public class BoldService : IBoldService
{
    private readonly string _apiKey;
    private readonly string _baseUrl;
    private readonly string _successUrl;
    private readonly string _failureUrl;

    public BoldService(IConfiguration configuration)
    {
        _apiKey = configuration["Bold:ApiKey"];
        _baseUrl = configuration["Bold:BaseUrl"];
        _successUrl = configuration["Bold:SuccessUrl"];
        _failureUrl = configuration["Bold:FailureUrl"];

        if (string.IsNullOrEmpty(_apiKey))
        {
            throw new Exception("Bold API key is not configured.");
        }
    }

    public string GenerateSignature(string orderId, string amount, string currency)
    {
        if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(amount) || string.IsNullOrEmpty(currency))
        {
            throw new ArgumentException("Missing required data for signature generation");
        }

        string rawData = $"{orderId}{amount}{currency}{_apiKey}";

        //string rawData = $"ORD175131337993929000COPydgILNf8rAjC2qsY-kTwUg";

        Console.WriteLine("🔐 rawData: " + rawData);

        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(rawData);
            byte[] hashBytes = sha256.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }

    public string GetSuccessUrl() => _successUrl;
    public string GetFailureUrl() => _failureUrl;
    public string GetBaseUrl() => _baseUrl;
}


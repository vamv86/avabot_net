namespace AiAgentApi.DTOs
{
    using System.Text.Json.Serialization;

    public class BoldConfirmationDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("subject")]
        public string Subject { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("spec_version")]
        public string SpecVersion { get; set; }

        [JsonPropertyName("time")]
        public long Time { get; set; }

        [JsonPropertyName("data")]
        public BoldConfirmationData Data { get; set; }

        [JsonPropertyName("datacontenttype")]
        public string Datacontenttype { get; set; }
    }

    public class BoldConfirmationData
    {
        [JsonPropertyName("payment_id")]
        public string PaymentId { get; set; }

        [JsonPropertyName("merchant_id")]
        public string MerchantId { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("amount")]
        public BoldConfirmationAmount Amount { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("metadata")]
        public BoldConfirmationMetadata Metadata { get; set; }

        [JsonPropertyName("bold_code")]
        public string BoldCode { get; set; }

        [JsonPropertyName("payer_email")]
        public string PayerEmail { get; set; }

        [JsonPropertyName("payment_method")]
        public string PaymentMethod { get; set; }
        public string? PayerPhone { get; set; }
    }

    public class BoldConfirmationAmount
    {
        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("total")]
        public decimal Total { get; set; }

        [JsonPropertyName("taxes")]
        public List<object> Taxes { get; set; }

        [JsonPropertyName("tip")]
        public decimal Tip { get; set; }
    }

    public class BoldConfirmationMetadata
    {
        [JsonPropertyName("reference")]
        public string Reference { get; set; }
    }


}

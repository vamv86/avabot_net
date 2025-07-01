namespace AiAgentApi.DTOs
{
    public class ConfirmationDto
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Subject { get; set; }
        public string Source { get; set; }
        public string Spec_Version { get; set; }
        public long Time { get; set; }
        public string Datacontenttype { get; set; }
        public Data Data { get; set; }
    }

    public class Data
    {
        public string Payment_Id { get; set; }
        public string Merchant_Id { get; set; }
        public string Created_At { get; set; }
        public Amount Amount { get; set; }
        public object User_Id { get; set; }
        public Metadata Metadata { get; set; }
        public string Bold_Code { get; set; }
        public string Payer_Email { get; set; }
        public string Payment_Method { get; set; }
        public Card Card { get; set; }
    }

    public class Amount
    {
        public string Currency { get; set; }
        public decimal Total { get; set; }
        public List<object> Taxes { get; set; }
        public decimal Tip { get; set; }
    }

    public class Metadata
    {
        public string Reference { get; set; }
    }

    public class Card
    {
        public string Brand { get; set; }
        public string Cardholder_Name { get; set; }
        public string Masked_Pan { get; set; }
    }

}

namespace PaymentService.Contracts.Models
{
    public class PaymentRequest
    {
        public string CardNumber { get; set; }
        
        public double Total { get; set; }
    }
}
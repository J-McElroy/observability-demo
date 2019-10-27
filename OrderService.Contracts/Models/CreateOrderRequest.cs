namespace OrderService.Contracts.Models
{
    public class CreateOrderRequest
    {
        public string ItemCode { get; set; }
        
        public string CardNumber { get; set; }
    }
}
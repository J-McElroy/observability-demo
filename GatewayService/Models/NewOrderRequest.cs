namespace GatewayService.Models
{
    public class NewOrderRequest
    {
        public string ItemCode { get; set; }
        
        public string CardNumber { get; set; }
    }
}
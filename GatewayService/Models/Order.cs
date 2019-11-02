using System.Collections.Generic;

namespace GatewayService.Models
{
    public class Order
    {
        public string Id { get; set; }
        
        public double Total { get; set; }
        
        public List<string> Items { get; set; }
    }
}
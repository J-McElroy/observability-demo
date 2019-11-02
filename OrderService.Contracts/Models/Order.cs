using System.Collections.Generic;

namespace OrderService.Contracts.Models
{
    public class Order
    {
        public string Id { get; set; }
        
        public double Total { get; set; }
        
        public List<string> Items { get; set; }
    }
}
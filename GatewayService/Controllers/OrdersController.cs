using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GatewayService.Models;
using Microsoft.AspNetCore.Mvc;
using OrderService.Contracts;
using OrderService.Contracts.Models;
using Serilog;
using Order = GatewayService.Models.Order;

namespace GatewayService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private static readonly ILogger Logger = Log.Logger.ForContext<OrdersController>();
        
        private readonly IOrderServiceClient _client;
        
        public OrdersController(IOrderServiceClient client)
        {
            _client = client;
        }
        
        // GET api/orders
        [HttpGet]
        public async Task<IEnumerable<Order>> GetOrders()
        {
            return (await _client.GetOrders()).Select(el => new Order()
            {
                Id = el.Id, 
                Items = el.Items,
                Total = el.Total
            });
        }
        
        [HttpGet("str/{id}")]
        public ActionResult<string> GetString(string id)
        {
            Logger.Information("{IntegerField}", id);
            return "value";
        }

        // POST api/orders
        [HttpPost]
        public Task<string> CreateOrder([FromBody] NewOrderRequest request)
        {
            return _client.CreateOrder(new CreateOrderRequest()
                {
                    CardNumber = request.CardNumber,
                    ItemCode = request.ItemCode
                }
            );
        }
    }
}
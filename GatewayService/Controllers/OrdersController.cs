using System.Collections.Generic;
using System.Threading.Tasks;
using GatewayService.Models;
using Microsoft.AspNetCore.Mvc;
using OrderService.Contracts;
using OrderService.Contracts.Models;
using Serilog;

namespace GatewayService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private static ILogger _logger = Log.Logger.ForContext<OrdersController>();
        
        private readonly IOrderServiceClient _client;
        
        public OrdersController(IOrderServiceClient client)
        {
            _client = client;
        }
        
        // GET api/orders
        [HttpGet]
        public Task<IEnumerable<string>> GetOrders()
        {
            return _client.GetOrders();
        }
        
        [HttpGet("str/{id}")]
        public ActionResult<string> GetString(string id)
        {
            _logger.Information("{IntegerField}", id);
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
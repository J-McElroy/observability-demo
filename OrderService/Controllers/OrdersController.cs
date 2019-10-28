using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Pipelines;
using Microsoft.AspNetCore.Mvc;
using OrderService.Contracts.Models;
using PaymentService.Contracts;
using PaymentService.Contracts.Models;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [MiddlewareFilter(typeof(RandomErrorPipeline))]
    public class OrdersController : ControllerBase
    {
        private readonly IPaymentServiceClient _client;

        public OrdersController(IPaymentServiceClient client)
        {
            _client = client;
        }
        
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] {"value12", "value22"};
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public Task<string> Post([FromBody] CreateOrderRequest request)
        {
            return _client.MakePayment(new PaymentRequest()
            {
                CardNumber = request.CardNumber,
                Total = 12.5
            });
        }
    }
}
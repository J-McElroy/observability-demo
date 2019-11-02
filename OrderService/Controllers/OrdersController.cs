using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
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
        private readonly Fixture _orderGenerator;

        public OrdersController(IPaymentServiceClient client)
        {
            _client = client;
            _orderGenerator = new Fixture();
            _orderGenerator.Customize<Order>(
                c => c.With(
                    x => x.Items, _orderGenerator.CreateMany<string>(new Random().Next(5)).ToList()));
        }

        [HttpGet]
        public ActionResult<IEnumerable<Order>> Get()
        {
            return Ok(_orderGenerator.CreateMany<Order>(new Random().Next(5)));
        }


        [HttpGet("{id}")]
        public ActionResult<Order> Get(int id)
        {
            return Ok(_orderGenerator.Create<Order>());
        }

        [HttpPost]
        public Task<string> Post([FromBody] CreateOrderRequest request)
        {
            return _client.MakePayment(new PaymentRequest()
            {
                CardNumber = request.CardNumber,
                Total = _orderGenerator.Create<double>()
            });
        }
    }
}
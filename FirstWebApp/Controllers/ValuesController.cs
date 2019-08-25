using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirstWebApp;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Core;

namespace TestWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private static ILogger _logger = Log.Logger.ForContext<ValuesController>();
        
        private readonly ISecondServiceClient _client;
        
        public ValuesController(ISecondServiceClient client)
        {
            _client = client;
        }
        
        // GET api/values
        [HttpGet]
        public Task<IEnumerable<string>> Get()
        {
            return _client.Get();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            _logger.Information("{IntegerField}", id);
            return "value";
        }
        
        [HttpGet("str/{id}")]
        public ActionResult<string> GetString(string id)
        {
            _logger.Information("{IntegerField}", id);
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
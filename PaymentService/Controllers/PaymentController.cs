using Microsoft.AspNetCore.Mvc;
using PaymentService.Contracts.Models;

namespace PaymentService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        // POST api/values
        [HttpPost]
        public ActionResult<string> Post([FromBody] PaymentRequest request)
        {
            return "Completed";
        }
    }
}
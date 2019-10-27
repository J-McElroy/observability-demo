using Common;
using Microsoft.Extensions.Configuration;

namespace PaymentService
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration)
            : base(configuration)
        {
        }

        public override string ServiceName => "PaymentService";
    }
}
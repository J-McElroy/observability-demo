using Common;

namespace GatewayService
{
    public class Program
    {
        private const string ServiceName = "GatewayService";

        public static void Main(string[] args)
        {
            var service = new WebHostRunner<Startup>(ServiceName, args);
            service.Run();
        }
    }
}
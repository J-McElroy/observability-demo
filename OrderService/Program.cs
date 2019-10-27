using Common;

namespace OrderService
{
    public class Program
    {
        private const string ServiceName = "OrderService";
        
        public static void Main(string[] args)
        {
            var service = new WebHostRunner<Startup>(ServiceName, args);
            service.Run();
        }
    }
}
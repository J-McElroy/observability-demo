using Common;

namespace FirstWebApp
{
    public class Program
    {
        private const string ServiceName = "FirstService";

        public static void Main(string[] args)
        {
            var service = new WebHostRunner<Startup>(ServiceName, args);
            service.Run();
        }
    }
}
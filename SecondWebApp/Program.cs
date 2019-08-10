using Common;

namespace SecondWebApp
{
    public class Program
    {
        private const string ServiceName = "SecondService";
        
        public static void Main(string[] args)
        {
            var service = new WebHostRunner<Startup>(ServiceName, args);
            service.Run();
        }
    }
}
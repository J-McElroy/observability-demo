using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PaymentService
{
    public class Program
    {
        private const string ServiceName = "PaymentService";

        public static void Main(string[] args)
        {
            var service = new WebHostRunner<Startup>(ServiceName, args);
            service.Run();
        }
    }
}
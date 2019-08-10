using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace FirstWebApp
{
    public interface ISecondServiceClient
    {
        [Get("/values")]
        Task<IEnumerable<string>> Get();
    }
}
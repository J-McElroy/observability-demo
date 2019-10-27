using System.Collections.Generic;
using System.Threading.Tasks;
using OrderService.Contracts.Models;
using Refit;

namespace OrderService.Contracts
{
    public interface IOrderServiceClient
    {
        [Get("/orders")]
        Task<IEnumerable<string>> GetOrders();

        [Post("/orders")]
        Task<string> CreateOrder(CreateOrderRequest request);
    }
}
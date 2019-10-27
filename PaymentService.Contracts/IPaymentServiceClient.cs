using System.Threading.Tasks;
using PaymentService.Contracts.Models;
using Refit;

namespace PaymentService.Contracts
{
    public interface IPaymentServiceClient
    {
        [Post("/payment")]
        Task<string> MakePayment(PaymentRequest request);
    }
}
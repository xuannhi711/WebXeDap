using WebXeDap.Models;
using WebXeDap.Models.VNPay;

namespace WebXeDap.Services
{
    public interface IVNPayServices
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);

        PaymentResponseModel PaymentExecute(IQueryCollection collections);
        
    }

}

using Microsoft.AspNetCore.Mvc;
using WebXeDap.Models.VNPay;
using WebXeDap.Services;

namespace WebXeDap.Controllers
{
    public class PaymentController : Controller
    {

        private readonly IVNPayServices _vnPayService;
        public PaymentController(IVNPayServices vnPayService)
        {

            _vnPayService = vnPayService;
        }

        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(url);
        }
        [HttpGet]
        public IActionResult PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            return Json(response);
        }


    }

}
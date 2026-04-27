using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using WebXeDap.Library;
using WebXeDap.Models.VNPay;

namespace WebXeDap.Services
{
	public class VNPayService : IVNPayServices
	{
		private readonly IConfiguration _configuration;

		public VNPayService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
		{
			string vnp_Returnurl = model.ReturnUrl; // Sử dụng ReturnUrl từ model
			string vnp_Url = _configuration["VNPay:Url"];
			string vnp_TmnCode = _configuration["VNPay:TmnCode"];
			string vnp_HashSecret = _configuration["VNPay:HashSecret"];

			var vnpay = new VnpayLibrary();

			vnpay.AddRequestData("vnp_Version", "2.1.0");
			vnpay.AddRequestData("vnp_Command", "pay");
			vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
			vnpay.AddRequestData("vnp_Amount", ((int)model.Amount * 100).ToString());
			vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
			vnpay.AddRequestData("vnp_CurrCode", "VND");
			vnpay.AddRequestData("vnp_IpAddr", context.Connection.RemoteIpAddress.ToString());
			vnpay.AddRequestData("vnp_Locale", "vn");
			vnpay.AddRequestData("vnp_OrderInfo", model.OrderDescription);
			vnpay.AddRequestData("vnp_OrderType", model.OrderType);
			vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
			vnpay.AddRequestData("vnp_TxnRef", model.OrderId);

			string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
			return paymentUrl;
		}

		public PaymentResponseModel PaymentExecute(IQueryCollection collections)
		{
			var vnpay = new VnpayLibrary();

			foreach (var (key, value) in collections)
			{
				if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
				{
					vnpay.AddResponseData(key, value);
				}
			}

			string vnp_HashSecret = _configuration["VNPay:HashSecret"];
			bool checkSignature = vnpay.ValidateSignature(
				vnpay.GetResponseData("vnp_SecureHash"),
				vnp_HashSecret
			);

			if (checkSignature)
			{
				return new PaymentResponseModel
				{
					OrderId = vnpay.GetResponseData("vnp_TxnRef"),
					PaymentId = vnpay.GetResponseData("vnp_TransactionNo"),
					TransactionId = vnpay.GetResponseData("vnp_TransactionNo"),
					VnPayResponseCode = vnpay.GetResponseData("vnp_ResponseCode"),
					PaymentMethod = vnpay.GetResponseData("vnp_CardType"),
					OrderDescription = vnpay.GetResponseData("vnp_OrderInfo"),
				};
			}
			return null;
		}
	}
}

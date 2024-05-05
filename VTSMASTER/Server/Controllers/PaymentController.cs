using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VTSMASTER.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentController : ControllerBase
	{
		private readonly IPaymentService _paymentService;

		public PaymentController(IPaymentService paymentService)
        {
			_paymentService = paymentService;
		}

		[HttpPost("checkout"), Authorize]
		public async Task<ActionResult<string>> CreateCheckoutSession()
		{
			var session = await _paymentService.CreateCheckoutSession();
			return Ok(session.Url);
		}

		[HttpPost]
		public async Task<ActionResult<ServiceResponse<bool>>> FulfillOrder()
		{
			var responce = await _paymentService.FulfillOrder(Request);
			if(!responce.Success)
				return BadRequest(responce.Message);

			return Ok(responce);
		}
    }
}

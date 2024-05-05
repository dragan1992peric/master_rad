using Stripe.Checkout;

namespace VTSMASTER.Server.Services.PaymentService
{
	public interface IPaymentService
	{
		Task<Session> CreateCheckoutSession();
		Task<ServiceResponse<bool>> FulfillOrder(HttpRequest request);
	}
}

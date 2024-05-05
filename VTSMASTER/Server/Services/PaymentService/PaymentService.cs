using Stripe;
using Stripe.Checkout;

namespace VTSMASTER.Server.Services.PaymentService
{
	public class PaymentService : IPaymentService
	{
		private readonly ICartService _cartService;
		private readonly IAuthService _authService;
		private readonly IOrderService _orderService;

		const string secret = "whsec_0e8b685dcc0b7752d6b4bd8927458bbd2655876b7df8e29261cc2e16c22d92cc";


        public PaymentService(ICartService cartService, IAuthService authService, IOrderService orderService)
        {
			StripeConfiguration.ApiKey = "sk_test_51PCzgW06D8b9rqryqLNf59ehQOS4GO2rWYYVMlPujMirKG1zkzH9EzxHyPf9lf2Vu9edmCSPFoN4DJS89PRlL78600lIpQgzlG";

			_cartService = cartService;
			_authService = authService;
			_orderService = orderService;
		}
        public async Task<Session> CreateCheckoutSession()
		{
			var products = (await _cartService.GetDbCartProducts()).Data;
			var lineItem = new List<SessionLineItemOptions>();
			products.ForEach(product => lineItem.Add(new SessionLineItemOptions
			{
				PriceData = new SessionLineItemPriceDataOptions
				{
					UnitAmountDecimal = product.Price * 100,
					Currency = "usd",
					ProductData = new SessionLineItemPriceDataProductDataOptions
					{
						Name = product.Title,
						Images = new List<string> { product.ImageUrl}
					}
				},
				Quantity = product.Quantity
			}));

			var options = new SessionCreateOptions
			{
				CustomerEmail = _authService.GetUserEmail(),
				ShippingAddressCollection = new SessionShippingAddressCollectionOptions
				{
					AllowedCountries = new List<string> { "US" }
				},
				PaymentMethodTypes = new List<string>
				{
					"card"
				},
				LineItems = lineItem,
				Mode = "payment",
				SuccessUrl = "https://localhost:7081/order-success",
				CancelUrl = "https://localhost:7081/cart"
			};

			var service = new SessionService();
			Session session = service.Create(options);
			return session;
		}

        public async Task<ServiceResponse<bool>> FulfillOrder(HttpRequest request)
        {
			var json = await new StreamReader(request.Body).ReadToEndAsync();
			try
			{
				var stripeEvent = EventUtility.ConstructEvent(
					json,
					request.Headers["Stripe-Signature"],
					secret
					);

				if(stripeEvent.Type == Events.CheckoutSessionCompleted)
				{
					var session = stripeEvent.Data.Object as Session;
					var user = await _authService.GetUserByEmail(session.CustomerEmail);
					await _orderService.PlaceOrder(user.Id);
				}

				return new ServiceResponse<bool> { Data = true };
			}
			catch (StripeException e)
			{
				return new ServiceResponse<bool>
				{
					Data = false,
					Success = false,
					Message = e.Message
				};
			}
        }
    }
}

using System.Security.Claims;

namespace VTSMASTER.Server.Services.OrderService
{
	public class OrderService : IOrderService
	{
		private readonly DataContext _context;
		private readonly ICartService _cartService;
		private readonly IAuthService _authService;

		public OrderService(DataContext context,
			ICartService cartService,
			IAuthService authService)
        {
			_context = context;
			_cartService = cartService;
			_authService = authService;
		}

        public async Task<ServiceResponse<OrderDetailsResponce>> GetOrderDetails(int orderId)
        {
            var response = new ServiceResponse<OrderDetailsResponce>();
			var order = await _context.Orders
				.Include(o => o.OrderItems)
				.ThenInclude(oi => oi.Product)
				.Include(o => o.OrderItems)
				.ThenInclude(oi => oi.ProductType)
				.Where(o => o.UserId == _authService.GetUserId() && o.Id == orderId)
				.OrderByDescending(o => o.OrderDate)
				.FirstOrDefaultAsync();

			if(order == null)
			{
				response.Success = false;
				response.Message = "није пронађено";
				return response;
			}

			var orderDetailsResponse = new OrderDetailsResponce
			{
				OrderDate = order.OrderDate,
				TotalPrice = order.TotalPrice,
				Products = new List<OrderDetailsProductResponce>()
			};

			order.OrderItems.ForEach(item =>
			orderDetailsResponse.Products.Add(new OrderDetailsProductResponce
			{
				ProductId = item.ProductId,
				ImageUrl = item.Product.ImageUrl,
				ProductType = item.ProductType.Name,
				Quantity = item.Quantity,
				Title = item.Product.Title,
				TotalPrice = item.TotalPrice
			}));

			response.Data = orderDetailsResponse;

			return response;

        }

        public async Task<ServiceResponse<List<OrderOverviewResponce>>> GetOrders()
		{
			var responce = new ServiceResponse<List<OrderOverviewResponce>>();
			var orders = await _context.Orders
				.Include(o => o.OrderItems)
				.ThenInclude(oi => oi.Product)
				.Where(o => o.UserId == _authService.GetUserId())
				.OrderByDescending(o => o.OrderDate)
				.ToListAsync();

			var orderResponce = new List<OrderOverviewResponce>();
			orders.ForEach(o => orderResponce.Add(new OrderOverviewResponce
			{
				Id = o.Id,
				OrderDate = o.OrderDate,
				TotalPrice = o.TotalPrice,
				Product = o.OrderItems.Count > 1 ?
					$"{o.OrderItems.First().Product.Title} and" +
					$"{o.OrderItems.Count - 1} more..." :
					o.OrderItems.First().Product.Title,
				ProductImageUrl = o.OrderItems.First().Product.ImageUrl
			}));

			responce.Data = orderResponce;

			return responce;
		}

		public async Task<ServiceResponse<bool>> PlaceOrder()
		{
			var products = (await _cartService.GetDbCartProducts()).Data;
			decimal totalPrice = 0;
			products.ForEach(product => totalPrice += product.Price * product.Quantity);

			var orderItems = new List<OrderItem>();
			products.ForEach(product => orderItems.Add(new OrderItem
			{
				ProductId = product.ProductId,
				ProductTypeId = product.ProductTypeId,
				Quantity = product.Quantity,
				TotalPrice = product.Price * product.Quantity
			}));

			var order = new Order
			{
				UserId = _authService.GetUserId(),
				OrderDate = DateTime.Now,
				TotalPrice = totalPrice,
				OrderItems = orderItems
			};

			_context.Orders.Add(order);

			_context.CartItems.RemoveRange(_context.CartItems.Where(ci => ci.UserId == _authService.GetUserId()));

			await _context.SaveChangesAsync();

			return new ServiceResponse<bool> { Data = true };
		}
	}
}

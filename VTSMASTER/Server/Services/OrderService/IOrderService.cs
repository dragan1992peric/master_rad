namespace VTSMASTER.Server.Services.OrderService
{
	public interface IOrderService
	{
		Task<ServiceResponse<bool>> PlaceOrder();
		Task<ServiceResponse<List<OrderOverviewResponce>>> GetOrders();
		Task<ServiceResponse<OrderDetailsResponce>> GetOrderDetails(int orderId);
	}
}

namespace VTSMASTER.Client.Services.OrderService
{
	public interface IOrderService
	{
		Task PlaceOrder();
		Task<List<OrderOverviewResponce>> GetOrders();
		Task<OrderDetailsResponce> GetOrderDetails(int orderId);
	}
}

﻿namespace VTSMASTER.Server.Services.OrderService
{
	public interface IOrderService
	{
		Task<ServiceResponse<bool>> PlaceOrder(int userId);
		Task<ServiceResponse<List<OrderOverviewResponce>>> GetOrders();
		Task<ServiceResponse<OrderDetailsResponce>> GetOrderDetails(int orderId);
	}
}

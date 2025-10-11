using OrderService.Domain.Entities;
using OrderService.Domain.Enum;

namespace OrderService.Domain.Repositories
{
    public interface IOrderRepository
    {

        Task<Order?> AddAsync(Order order);
        Task<Order?> GetByIdAsync(Guid orderId);
        Task<List<Order>> GetByUserIdAsync(Guid userId, int pageNumber = 1, int pageSize = 10);
        Task<bool> ChangeOrderStatusAsync(Guid orderId, OrderStatusEnum newStatusId, string? changedBy = null, string? remarks = null);
        Task<List<OrderStatusHistory>> GetOrderStatusHistoryAsync(Guid orderId);
        Task<bool> DeleteAsync(Guid orderId);
        Task<bool> ExistsAsync(Guid orderId);

        Task<(List<Order> Items, int TotalCount)> GetOrdersWithFiltersAsync
            (
                OrderStatusEnum? status = null,
                DateTime? fromDate = null,
                DateTime? toDate = null,
                string? searchOrderNumber = null,
                int pageNumber = 1,
                int pageSize = 20
            );

        //// Get a specific order by its unique ID (Customer or Admin).
        //Task<Order?> GetByIdAsync(Guid orderId);

        //// Get paginated list of orders for a specific user (most recent first) [Customer].
        //Task<List<Order>> GetByUserIdAsync(Guid userId, int pageNumber = 1, int pageSize = 10);

        //// Get paginated list of all orders (Admin, Reporting, most recent first).
        //Task<List<Order>> GetAllAsync(int pageNumber = 1, int pageSize = 20);

        //// Add a new order (Customer).
        //Task<Order?> AddAsync(Order order);

        //// Changes the status of the order with optional remarks, (Admin or System).
        //// Also records the status change in OrderStatusHistory for audit trail.
        //Task<bool> ChangeOrderStatusAsync(Guid orderId, OrderStatusEnum newStatusId, string? changedBy = null, string? remarks = null);


        //// Gets the full status history for an order (Customer, Admin, Reporting).
        //Task<List<OrderStatusHistory>> GetOrderStatusHistoryAsync(Guid orderId);

        //// Delete an order by ID (Admin only).
        //Task DeleteAsync(Guid orderId);

        //// Check if an order exists by ID (internal, utility).
        //Task<bool> ExistsAsync(Guid orderId);

        //// Get order with full details (OrderItems, Cancellations, etc.). Used by Admin and Customer for details page.
        //Task<Order?> GetOrderWithDetailsAsync(Guid orderId);

        //// Get orders with a specific status (Admin, Reporting).
        //Task<List<Order>> GetByStatusAsync(OrderStatusEnum orderStatusId, int pageNumber = 1, int pageSize = 20);

        //// Get orders within a date range (Reporting, Admin).
        //Task<List<Order>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, int pageNumber = 1, int pageSize = 20);

    }
}

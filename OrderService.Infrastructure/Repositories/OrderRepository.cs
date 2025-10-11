using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Domain.Enum;
using OrderService.Domain.Repositories;
using OrderService.Infrastructure.Persistence;

namespace OrderService.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _context;


        // Define allowed transitions dictionary using OrderStatusEnum
        private static readonly Dictionary<OrderStatusEnum, List<OrderStatusEnum>> AllowedTransitions = new()
        {
            //Pending → Confirmed, Cancelled
            { OrderStatusEnum.Pending, new List<OrderStatusEnum> { OrderStatusEnum.Confirmed, OrderStatusEnum.Cancelled } },
            
            //Confirmed → Packed, Cancelled
            { OrderStatusEnum.Confirmed, new List<OrderStatusEnum> { OrderStatusEnum.Packed, OrderStatusEnum.Cancelled } },
            
            //Packed → Shipped, Cancelled
            { OrderStatusEnum.Packed, new List<OrderStatusEnum> { OrderStatusEnum.Shipped, OrderStatusEnum.Cancelled } },
            
            //Shipped → Delivered, Cancelled
            { OrderStatusEnum.Shipped, new List<OrderStatusEnum> { OrderStatusEnum.Delivered, OrderStatusEnum.Cancelled } },
            
            //Delivered → Returned
            { OrderStatusEnum.Delivered, new List<OrderStatusEnum> { OrderStatusEnum.Returned } },  
            
            //Cancelled & Returned → terminal
            { OrderStatusEnum.Cancelled, new List<OrderStatusEnum>() },  // Terminal state
            { OrderStatusEnum.Returned, new List<OrderStatusEnum>() }    // Terminal state
        };

        public OrderRepository(OrderDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

        }

        // Add a new order (Customer)

        public async Task<Order?> AddAsync(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            await _context.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }
        // Get order with full details (OrderItems, Cancellations, etc.) for details page (Admin and 
        public async Task<Order?> GetByIdAsync(Guid orderId)
        {
            Order? order = await _context.Orders.AsNoTracking()
                .Include(o=>o.OrderItems)
                .Include(o=>o.OrderCancellations)
                    .ThenInclude(c=>c.CancellationItems)
                .Include(o=>o.OrderReturns)
                    .ThenInclude(r=>r.ReturnItems)
                .Include(r=>r.Refunds)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            return order ?? new Order();

        }
        // Get paginated list of orders for a specific user (most recent first) [Customer]
        public async Task<List<Order>> GetByUserIdAsync(Guid userId, int pageNumber = 1, int pageSize = 20)
        {
            var orders = await _context.Orders.Where(x => x.UserId == userId)
                .AsNoTracking()
                .OrderByDescending(o => o.OrderDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return orders;
        }
        // Changes the status of the order with optional remarks, also records status history
        public async Task<bool> ChangeOrderStatusAsync(Guid orderId, OrderStatusEnum newStatusId, string? changedBy = null, string? remarks = null)
        {
            var orderHistory = await _context.Orders.Include(o => o.OrderStatusHistories)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            if (orderHistory == null) return false;
            if (orderHistory.OrderStatusId == (int)newStatusId) return true;

            var oldStatusId = orderHistory.OrderStatusId;
            var oldStatus = (OrderStatusEnum)oldStatusId;

            var newStatus = newStatusId;

            // Validate transition
            if (!AllowedTransitions.TryGetValue(oldStatus, out var validateNextStatuses) || !validateNextStatuses.Contains(newStatus))
            {
                throw new InvalidOperationException($"Transition from {oldStatus} to {newStatus} is not allowed.");
            }
            // Update the order status
            orderHistory.OrderStatusId = (int)newStatusId;
            // If new status is Delivered, set the DeliveryDate
            if (newStatusId == OrderStatusEnum.Delivered)
            {
                orderHistory.DeliveryDate = DateTime.UtcNow;
            }
            // Record status history
            orderHistory.OrderStatusHistories.Add(new OrderStatusHistory
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                OldStatusId = oldStatusId,
                NewStatusId = (int)newStatusId,
                ChangedBy = changedBy,
                Remarks = remarks,
                ChangedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
            return true;
        }
        // Gets the full status history for an order (Customer, Admin, Reporting)
        public async Task<List<OrderStatusHistory>> GetOrderStatusHistoryAsync(Guid orderId)
        {
            return  await _context.OrderStatusHistories.Where(o => o.OrderId == orderId)
                .AsNoTracking()
                .OrderByDescending(x => x.ChangedAt)
                .ToListAsync();           
        }
        // Delete an order by ID (Admin only)
        public async Task<bool> DeleteAsync(Guid orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) return false;
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

        // Check if an order exists by ID (internal utility)
        public async Task<bool> ExistsAsync(Guid orderId)
        {
            return await _context.Orders.AsNoTracking().AnyAsync(o => o.Id == orderId);
            
        }
        // Get order with full details (OrderItems, Cancellations, etc.) for details page (Admin and Customer)
        public async Task<(List<Order> Items, int TotalCount)> GetOrdersWithFiltersAsync
        (
            OrderStatusEnum? status=null,
            DateTime? fromDate=null,
            DateTime? toDate=null,
            string? searchOrderNumber=null,
            int pageNumber=1, 
            int pageSize=20
        )
        {
            var query = _context.Orders.AsQueryable();
            if(status.HasValue)
                query = query.Where(x=>x.OrderStatusId==(int)status.Value);
            if(fromDate.HasValue)
                query=query.Where(x=>x.CreatedAt>=fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(x => x.CreatedAt <= toDate.Value);
            if(!string.IsNullOrWhiteSpace(searchOrderNumber))
                query=query.Where(x=> EF.Functions.Like(x.OrderNumber,$"%{searchOrderNumber}%"));
            var totalCount = await query.CountAsync();

            var items=await query.AsNoTracking()
                .Skip((pageNumber-1)*pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (items, totalCount);
        }
    }
}


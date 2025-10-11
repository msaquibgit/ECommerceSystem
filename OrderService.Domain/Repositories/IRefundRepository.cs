using OrderService.Domain.Entities;
using OrderService.Domain.Enum;

namespace OrderService.Domain.Repositories
{
    public interface IRefundRepository
    {
        Task<Refund> AddAsync(Refund refund);
        Task<Refund?> GetByIdAsync(Guid refundId);
        Task<Refund> UpdateStatusAsync(Guid refundId, RefundStatusEnum newStatus, string processedBy, string? remarks = null);
        Task DeleteAsync(Guid refundId);
        Task<bool> ExistsAsync(Guid refundId);
        Task<Refund?> GetByCancellationIdAsync(Guid cancellationId);
        Task<Refund?> GetByReturnIdAsync(Guid returnId);
        Task<(List<Refund> Items, int TotalCount)> GetByOrderIdAsync(Guid orderId, int pageNumber = 1, int pageSize = 20);
        Task<(List<Refund> Items, int TotalCount)> GetRefundsWithFiltersAsync(
            Guid? userId = null,
            RefundStatusEnum? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int pageNumber = 1,
            int pageSize = 20);







        //// Get a refund request by its unique ID (Customer or Admin).
        //Task<Refund?> GetByIdAsync(Guid refundId);

        //// Get all refunds for a specific order (Customer/My Orders).
        //Task<List<Refund>> GetByOrderIdAsync(Guid orderId);

        //// Add a new refund request (Customer, usually system initiated).
        //Task<Refund?> AddAsync(Refund refund);

        //// Update an existing refund (Customer can view status, usually admin updates).
        //Task<Refund?> UpdateAsync(Refund refund);

        //// Get all refund items linked to a refund (Admin or Customer).
        //Task<List<RefundItem>> GetRefundItemsByRefundIdAsync(Guid refundId);

        //// Check if a refund exists by ID.
        //Task<bool> ExistsAsync(Guid refundId);

        //// Get paginated list of all refunds (Admin, Reporting).
        //Task<List<Refund>> GetAllAsync(int pageNumber = 1, int pageSize = 20);

        //// Get refunds by status (Admin, Reporting).
        //Task<List<Refund>> GetByStatusAsync(RefundStatusEnum refundStatusId, int pageNumber = 1, int pageSize = 20);

    }
}

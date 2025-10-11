using OrderService.Domain.Entities;
using OrderService.Domain.Enum;

namespace OrderService.Domain.Repositories
{
    public interface IReturnRepository
    {
        Task<Return?> GetByIdAsync(Guid returnId);
        Task<List<Return>> GetReturnsByOrderIdAsync(Guid orderId);
        Task<Return?> AddAsync(Return returnRequest);
        Task<Return?> UpdateAsync(Return returnRequest);
        Task DeleteAsync(Guid returnId);
        Task<List<Guid>> GetReturnedOrderItemIdsAsync(Guid orderId);
        Task ApproveAsync(Return updatedReturn, string approvedBy, decimal refundableAmount, string? remarks = null);
        Task RejectAsync(Guid returnId, string rejectedBy, string? remarks = null);
        Task<List<ReturnItem>> GetReturnItemsByReturnIdAsync(Guid returnId);
        Task<bool> ExistsAsync(Guid returnId);
        Task<(List<Return> Items, int TotalCount)> GetReturnsWithFiltersAsync(
            Guid? userId = null,
            ReturnStatusEnum? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int pageNumber = 1,
            int pageSize = 20);




        //// Get a return request by its unique ID (Customer or Admin).
        //Task<Return?> GetByIdAsync(Guid returnId);

        //// Get all returns for a specific order (Customer/My Orders).
        //Task<List<Return>> GetByOrderIdAsync(Guid orderId);

        //// Add a new return request (Customer).
        //Task<Return?> AddAsync(Return orderReturn);

        //// Update an existing return (Customer can update reason before admin review).
        //Task<Return?> UpdateAsync(Return orderReturn);

        //// Delete a return request by ID (Customer or Admin).
        //Task DeleteAsync(Guid returnId);

        //// Approve a return request (Admin).
        //Task ApproveAsync(Guid returnId, string approvedBy, decimal refundableAmount, string? remarks = null);

        //// Reject a return request (Admin).
        //Task RejectAsync(Guid returnId, string rejectedBy, string? remarks = null);

        //// Get all return items linked to a return (Admin or Customer).
        //Task<List<ReturnItem>> GetReturnItemsByReturnIdAsync(Guid returnId);

        //// Check if a return exists by ID.
        //Task<bool> ExistsAsync(Guid returnId);

        //// Get paginated list of all returns (Admin, Reporting).
        //Task<List<Return>> GetAllAsync(int pageNumber = 1, int pageSize = 20);

        //// Get returns by status (Admin, Reporting).
        //Task<List<Return>> GetByStatusAsync(ReturnStatusEnum returnStatusId, int pageNumber = 1, int pageSize = 20);

        //// Get returns for a user (Customer).
        //Task<List<Return>> GetByUserIdAsync(Guid userId, int pageNumber = 1, int pageSize = 20);

        //// Get returns by date range (Admin, Reporting).
        //Task<List<Return>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, int pageNumber = 1, int pageSize = 20);

    }
}

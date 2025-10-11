using OrderService.Domain.Entities;
using OrderService.Domain.Entities.Master;
using OrderService.Domain.Enum;

namespace OrderService.Domain.Repositories
{
    public interface IMasterDataRepository
    {
        Task<List<CancellationPolicy>> GetCancellationPoliciesAsync();
        Task<CancellationPolicy?> GetCancellationPolicyByIdAsync(int id);
        Task<CancellationPolicy?> GetActiveCancellationPolicyAsync();

        Task<List<ReturnPolicy>> GetReturnPoliciesAsync();
        Task<ReturnPolicy?> GetReturnPolicyByIdAsync(int id);
        Task<ReturnPolicy?> GetActiveReturnPolicyAsync();

        Task<List<Discount>> GetDiscountsAsync();
        Task<Discount?> GetDiscountByIdAsync(int id);
        Task<List<Discount>> GetActiveDiscountsAsync(DateTime asOfDate);

        Task<List<Tax>> GetTaxesAsync();
        Task<Tax?> GetTaxByIdAsync(int id);
        Task<List<Tax>> GetActiveTaxesAsync(DateTime asOfDate);


    }
}

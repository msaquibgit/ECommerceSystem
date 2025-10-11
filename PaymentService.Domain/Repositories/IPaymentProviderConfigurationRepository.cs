using PaymentService.Domain.Entities.Master;

namespace PaymentService.Domain.Repositories
{
    public interface IPaymentProviderConfigurationRepository
    {
        public interface IPaymentProviderConfigurationRepository
        {
            Task<PaymentProviderConfigurationMaster?> GetActiveConfigurationByProviderAsync(int gatewayProviderId, int environmentId);
        }
    }
}

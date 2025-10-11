namespace OrderService.Domain.Enum
{
    public enum RefundStatusEnum
    {
        Pending = 1,               // Refund requested but not yet processed
        Processing = 2,          // Refund is being processed
        Completed = 3,          // Refund completed successfully
        Failed = 4,                  // Refund failed (e.g., payment gateway error)
        Cancelled = 5            // Refund request cancelled

    }
}

namespace OrderService.Contract.Enum
{
    public enum NotificationTypeEnum
    {
        OrderPlaced,
        PaymentSuccess,
        PaymentFailure,
        OrderCancelled,
        RefundInitiated,
        RefundCompleted,
        ReturnRequested,
        ReturnApproved,
        ReturnRejected,
        OrderStatusChanged,

    }
}

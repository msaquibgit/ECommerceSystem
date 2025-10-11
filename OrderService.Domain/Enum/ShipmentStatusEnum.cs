namespace OrderService.Domain.Enum
{
    public enum ShipmentStatusEnum
    {
        Pending = 1,
        Shipped = 2,
        InTransit = 3,
        OutForDelivery = 4,
        Delivered = 5,
        Returned = 6,
        Cancelled = 7
    }
}

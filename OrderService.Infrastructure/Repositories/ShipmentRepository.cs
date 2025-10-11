using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Domain.Enum;
using OrderService.Domain.Repositories;
using OrderService.Infrastructure.Persistence;

namespace OrderService.Infrastructure.Repositories
{
    public class ShipmentRepository:IShipmentRepository
    {
        private readonly OrderDbContext _context;

        public ShipmentRepository(OrderDbContext context)
        {
            _context = context??throw new ArgumentNullException(nameof(context));
        }
        private static readonly Dictionary<ShipmentStatusEnum, List<ShipmentStatusEnum>> AllowedTransitions = new()
        {
            { ShipmentStatusEnum.Pending, new List<ShipmentStatusEnum> { ShipmentStatusEnum.Shipped, ShipmentStatusEnum.Cancelled } },
            { ShipmentStatusEnum.Shipped, new List<ShipmentStatusEnum> { ShipmentStatusEnum.InTransit, ShipmentStatusEnum.Cancelled } },
            { ShipmentStatusEnum.InTransit, new List<ShipmentStatusEnum> { ShipmentStatusEnum.OutForDelivery, ShipmentStatusEnum.Cancelled } },
            { ShipmentStatusEnum.OutForDelivery, new List<ShipmentStatusEnum> { ShipmentStatusEnum.Delivered, ShipmentStatusEnum.Returned } },
            { ShipmentStatusEnum.Delivered, new List<ShipmentStatusEnum>() },
            { ShipmentStatusEnum.Cancelled, new List<ShipmentStatusEnum>() },
            { ShipmentStatusEnum.Returned, new List<ShipmentStatusEnum>() }
        };

        public async Task<Shipment> AddAsync(Shipment shipment,string location)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));
            shipment.IsDeleted = false;
            shipment.ShipmentStatusId = (int)ShipmentStatusEnum.Pending;
            shipment.CreatedAt = DateTime.UtcNow;
            shipment.UpdatedAt = null;
            await _context.Shipments.AddAsync(shipment);
            // Add initial status history record
            var history = new ShipmentStatusHistory
            {
                Id = Guid.NewGuid(),
                ShipmentId=shipment.Id,
                OldStatusId=shipment.ShipmentStatusId,
                NewStatusId=shipment.ShipmentStatusId,
                ChangedAt=DateTime.UtcNow,
                ChangedBy=null,
                Remarks="Initial Status",
                Location=location

            };
            await _context.ShipmentStatusHistories.AddAsync(history);
            await _context.SaveChangesAsync();
            return shipment;

        }
        public async Task<List<Shipment>> GetByOrderIdAsync(Guid orderId)
        {
            return await _context.Shipments.AsNoTracking()
                .Where(x => x.OrderId == orderId && !x.IsDeleted)
                .Include(i => i.ShipmentItems)
                .Include(s => s.ShipmentStatus)
                .OrderByDescending(s => s.EstimatedDeliveryDate)
                .ToListAsync();
        }
        public async Task<Shipment?> GetByIdAsync(Guid shipmentId)
        {
            if (shipmentId == Guid.Empty)
                throw new ArgumentException("Invalid shipment ID.", nameof(shipmentId));
            return await _context.Shipments
                .AsNoTracking()
                .Include(o => o.ShipmentStatus)
                .Include(s => s.ShipmentItems)
                .FirstOrDefaultAsync(x=>x.Id==shipmentId && !x.IsDeleted);

        }
        public async Task<Shipment?> UpdateAsync(Shipment shipment, string? changedBy = null, string? remarks = null, string? location = null)
        {
            if(shipment == null)
                throw new ArgumentNullException(nameof(shipment));
            var existingShipment = await _context.Shipments.FirstOrDefaultAsync(x => x.Id == shipment.Id && !x.IsDeleted);
            if (existingShipment == null) return null!;

            var currentStatus=(ShipmentStatusEnum)existingShipment.ShipmentStatusId;
            var newStatus=(ShipmentStatusEnum)shipment.ShipmentStatusId;
            if (currentStatus != newStatus)
            {
                if (!AllowedTransitions.TryGetValue(currentStatus, out var validNext) || !validNext.Contains(newStatus))
                {
                    throw new InvalidOperationException($"Invalid status transition from {currentStatus} to {newStatus}.");
                }
                // Update shipment status
                existingShipment.ShipmentStatusId = shipment.ShipmentStatusId;
                // Add status history record
                var history = new ShipmentStatusHistory
                {
                    Id = Guid.NewGuid(),
                    ShipmentId = existingShipment.Id,
                    OldStatusId = (int)currentStatus,
                    NewStatusId = (int)newStatus,
                    ChangedBy = changedBy,
                    ChangedAt = DateTime.UtcNow,
                    Remarks = remarks,
                    Location = location
                };
                await _context.ShipmentStatusHistories.AddAsync(history);
            }
                // Update other fields
                existingShipment.CarrierName = shipment.CarrierName;
                existingShipment.TrackingNumber = shipment.TrackingNumber;
                existingShipment.EstimatedDeliveryDate = shipment.EstimatedDeliveryDate;
                existingShipment.DeliveredAt = shipment.DeliveredAt;
                existingShipment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return existingShipment;            
        }
        public async Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber)) return null;
            
            return await _context.Shipments.AsNoTracking()
                .Include(s=>s.ShipmentStatus)
                .Include(s=>s.ShipmentItems)
                .Include(s=>s.ShipmentStatusHistories)
                .OrderByDescending(h=>h.UpdatedAt)
                .FirstOrDefaultAsync(x=>x.TrackingNumber==trackingNumber);
            
        }
    }
}

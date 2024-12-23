using Asp.NETCoreApi.Data;
using Asp.NETCoreApi.Dto;
using Asp.NETCoreApi.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Asp.NETCoreApi.Repositories {

    public class OrderRepository : IOrderRepository {
        private readonly MyDbContext _context;

        public OrderRepository (MyDbContext context) {
            _context = context;
        }


        public async Task<MesAndStaDto> Order (List<int> sizeIds, string userId, int paymentId, int deliverId) {
            // Validate inputs
            if (sizeIds == null || !sizeIds.Any()) {
                return new MesAndStaDto {
                    Status = 400,
                    Message = "Size IDs cannot be empty."
                };
            }

            if (string.IsNullOrEmpty(userId)) {
                return new MesAndStaDto {
                    Status = 400,
                    Message = "User ID is required."
                };
            }

            var paymentExists = await _context.Payments.AnyAsync(p => p.PaymentId == paymentId);
            if (!paymentExists) {
                return new MesAndStaDto {
                    Status = 400,
                    Message = "Invalid Payment ID. Payment not found in the database."
                };
            }

            // Check if deliverId exists in the database
            var deliverExists = await _context.Delivers.FirstOrDefaultAsync(d => d.DeliverId == deliverId);
            if (deliverExists == null) {
                return new MesAndStaDto {
                    Status = 400,
                    Message = "Invalid Deliver ID. Deliver option not found in the database."
                };
            }

            // Fetch user cart items for given sizeIds
            var itemsToBuy = await _context.ToBuyLaters
                .Where(x => sizeIds.Contains(x.SizeId) && x.UserId == userId)
                .Include(x => x.Size) // Include Size
                    .ThenInclude(size => size.Color) // Include associated Color
                        .ThenInclude(color => color.Product) // Include associated Product
                .ToListAsync();

            if (!itemsToBuy.Any()) {
                return new MesAndStaDto {
                    Status = 404,
                    Message = "No items found for the given size IDs."
                };
            }

            // Validate stock availability
            foreach (var item in itemsToBuy) {
                if (item.Quantity > item.Size.Stock) {
                    return new MesAndStaDto {
                        Status = 409, // Conflict status
                        Message = $"Insufficient stock for Size ID: {item.SizeId}. Requested: {item.Quantity}, Available: {item.Size.Stock}"
                    };
                }
            }

            // Calculate total price using Product.Price
            var totalPrice = itemsToBuy.Sum(item =>
                item.Quantity * item.Size.Color.Product.Price * ( 1 - (decimal) ( item.Size.Color.Product.Discount / 100 ) ))
                + (decimal) deliverExists.Fee;

            // Create the order
            var order = new Order {
                OrderDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                OrderStatus = "Pending",
                Address = "Sample Address", // Replace with actual address if provided
                TotalPrice = totalPrice,
                UserId = userId, // Assuming UserId is an int in the Order model
                PaymentId = paymentId,
                DeliverId = deliverId
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Deduct stock and create order details
            foreach (var item in itemsToBuy) {
                // Deduct stock
                item.Size.Stock -= item.Quantity;

                // Create order detail
                var orderDetail = new OrderDetail {
                    OrderId = order.OrderId,
                    SizeId = item.SizeId,
                    Quantity = item.Quantity,
                    Price = item.Size.Color.Product.Price  // Use Product.Price here via Color
                };

                _context.OrderDetails.Add(orderDetail);
            }

            // Remove the processed items from ToBuyLaters
            _context.ToBuyLaters.RemoveRange(itemsToBuy);

            await _context.SaveChangesAsync();

            return new MesAndStaDto {
                Status = 200, // Success
                Message = $"Order {order.OrderId} created successfully."
            };
        }




    }
}

using Asp.NETCoreApi.Data;
using Asp.NETCoreApi.Dto;
using Asp.NETCoreApi.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Asp.NETCoreApi.Repositories {
    public class OrderHistory : IOrderHistory {
        private readonly MyDbContext _context;

        public OrderHistory (MyDbContext context) {
            _context = context;
        }

        public async Task<List<HistoryOrderDto>> GetOrdersByUserId (string userId) {
            // Lấy danh sách đơn hàng dựa trên UserId
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Select(o => new HistoryOrderDto {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    OrderStatus = o.OrderStatus,
                    TotalPrice = o.TotalPrice,
                    // Lấy tên của phương thức thanh toán từ PaymentId
                    PaymentMethod = _context.Payments
                                            .Where(p => p.PaymentId == o.PaymentId)
                                            .Select(p => p.MeThodPayment)
                                            .FirstOrDefault() ?? "Unknown Payment Method", // Provide a default value for null
                                                                                           // Lấy tên của phương thức giao hàng từ DeliverId
                    DeliverMethod = _context.Delivers
                                            .Where(d => d.DeliverId == o.DeliverId)
                                            .Select(d => d.DeliverType)
                                            .FirstOrDefault() ?? "Unknown Delivery Method" // Provide a default value for null
                })
                .ToListAsync();

            return orders;
        }

        public async Task<List<ProductDto>> GetProductsByOrderIdAsync (int orderId, string userId) {

            var order = await _context.Orders
            .Where(o => o.OrderId == orderId && o.UserId == userId)
            .FirstOrDefaultAsync();

            if (order == null) {
                throw new UnauthorizedAccessException("You do not have access to this order.");
            }
            // Lấy chi tiết đơn hàng dựa trên OrderId
            var orderDetails = await _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .Include(od => od.Size) // Bao gồm thông tin về Size
                    .ThenInclude(size => size.Color) // Bao gồm thông tin về Color
                        .ThenInclude(color => color.Product) // Bao gồm thông tin về Product
                .Include(od => od.Size)
                    .ThenInclude(size => size.Color.Images) // Bao gồm hình ảnh của màu sắc
                .ToListAsync();

            // Lấy danh sách các mục đã chọn trong giỏ hàng (ToBuyLaters) của người dùng
            var toBuyLaters = await _context.ToBuyLaters
                .Where(t => orderDetails.Select(od => od.SizeId).Contains(t.SizeId))
                .ToListAsync();

            // Ánh xạ thông tin từ OrderDetails sang ProductDto
            var productDtos = orderDetails.Select(od => new ProductDto {
                ProductDtoId = od.Size.Color.Product.ProductId, // Lấy ProductId từ Size.Color.Product
                Name = od.Size.Color.Product.Name, // Tên sản phẩm
                Price = od.Price, // Giá mỗi sản phẩm trong đơn hàng
                Discount = od.Size.Color.Product.Discount, // Giảm giá của sản phẩm
                ColorDtos = new List<ColorDto> {
            new ColorDto
            {
                ColorDtoId = od.Size.Color.ColorId, // Mã màu
                Name = od.Size.Color.Name, // Tên màu
                HexCode = od.Size.Color.HexCode, // Mã màu Hex
                SizeDtos = new List<SizeDto> {
                    new SizeDto
                    {
                        SizeDtoId = od.Size.SizeId, // Mã kích thước
                        Name = od.Size.Name, // Tên kích thước
                        Stock = od.Size.Stock, // Số lượng tồn kho
                        QuantityOrder = od.Quantity // Số lượng đã đặt trong đơn hàng
                    }
                },
                ImageDtos = od.Size.Color.Images.Select(image => new ImageDto
                {
                    ImageDtoId = image.ImageId, // Mã hình ảnh
                    Data = image.Data // Dữ liệu hình ảnh (nếu cần)
                }).ToList()
            }
        }
            }).ToList();

            return productDtos;
        }

    }
}

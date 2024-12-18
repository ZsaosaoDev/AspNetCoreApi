using Asp.NETCoreApi.Data;
using Asp.NETCoreApi.Dto;
using Asp.NETCoreApi.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Asp.NETCoreApi.Repositories {
    public class ToBuyLaterRepository : IToBuyLaterRepository {

        private readonly MyDbContext _context;

        public ToBuyLaterRepository (MyDbContext context) {
            _context = context;
        }

        public async Task<string> SaveToBuyLater (int sizeId, string userId) {
            try {
                // Check if the size exists
                var sizeExists = await _context.Sizes.AnyAsync(s => s.SizeId == sizeId);
                if (!sizeExists) {
                    return "Size does not exist";
                }

                // Check if the user already has this size in their ToBuyLater list
                var existingToBuyLater = await _context.ToBuyLaters
                    .FirstOrDefaultAsync(t => t.UserId == userId && t.SizeId == sizeId);

                if (existingToBuyLater != null) {
                    // If it exists, increment the quantity
                    existingToBuyLater.Quantity++;
                }
                else {
                    // If it doesn't exist, create a new entry with quantity = 1
                    var toBuyLater = new ToBuyLater {
                        SizeId = sizeId,
                        UserId = userId,
                        Quantity = 1 // Start with quantity 1
                    };

                    _context.ToBuyLaters.Add(toBuyLater);
                }

                // Save the changes to the database
                var result = await _context.SaveChangesAsync();

                // Return success message if changes were saved
                if (result > 0) {
                    return "Saved successfully";
                }
                else {
                    return "No changes were made";
                }
            }
            catch (Exception ex) {
                // Log the exception and return a friendly error message
                // You can log the exception using a logger
                Console.WriteLine($"Error: {ex.Message}");
                return "An error occurred while saving the item. Please try again.";
            }
        }



        public async Task<List<ProductDto>> GetProductsWithSelectedColors (string userId) {
            // Fetch selected SizeIds for the user from the ToBuyLater table
            var toBuyLaters = await _context.ToBuyLaters
                .Where(t => t.UserId == userId)
                .ToListAsync();

            var selectedSizeIds = toBuyLaters
                .Select(t => t.SizeId)
                .Distinct()
                .ToList();

            // Nếu không có SizeId nào được chọn, trả về danh sách rỗng
            if (!selectedSizeIds.Any()) {
                return new List<ProductDto>();
            }

            // Fetch all products that have colors and sizes matching the selected sizes
            var products = await _context.Products
                .Include(p => p.Colors)
                    .ThenInclude(c => c.Sizes)
                .Include(p => p.Colors)
                    .ThenInclude(c => c.Images)
                .Where(p => p.Colors.Any(c => c.Sizes.Any(s => selectedSizeIds.Contains(s.SizeId))))
                .ToListAsync();

            // Map products to ProductDto and filter/mark selected sizes and colors
            var productDtos = products.Select(product => new ProductDto {
                ProductDtoId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                Discount = product.Discount,
                ColorDtos = product.Colors.Select(color => new ColorDto {
                    ColorDtoId = color.ColorId,
                    Name = color.Name,
                    HexCode = color.HexCode,
                    IsPreviouslySelected = color.Sizes.Any(size => selectedSizeIds.Contains(size.SizeId)), // Check if any size matches
                    SizeDtos = color.Sizes
                        .Where(size => selectedSizeIds.Contains(size.SizeId)) // Only take selected sizes
                        .Select(size => new SizeDto {
                            SizeDtoId = size.SizeId,
                            Name = size.Name,
                            Stock = size.Stock,
                            QuantityOrder = toBuyLaters
                                .Where(t => t.SizeId == size.SizeId)
                                .Sum(t => t.Quantity) // Set QuantityOrder per size
                        }).ToList(),
                    ImageDtos = color.Images.Select(image => new ImageDto {
                        ImageDtoId = image.ImageId,
                        Data = image.Data
                    }).ToList()
                }).ToList()
            }).ToList();

            return productDtos;
        }







    }

}

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

        public async Task<MesAndStaDto> SaveToBuyLater (int sizeId, string userId) {
            try {
                // Check if the size exists and fetch its details
                var size = await _context.Sizes.FirstOrDefaultAsync(s => s.SizeId == sizeId);
                if (size == null) {
                    return new MesAndStaDto("Size does not exist", 404); // 404 for Not Found
                }

                // Check if the stock is sufficient to add an item


                // Check if the user already has this size in their ToBuyLater list
                var existingToBuyLater = await _context.ToBuyLaters
                    .FirstOrDefaultAsync(t => t.UserId == userId && t.SizeId == sizeId);

                if (existingToBuyLater?.Quantity + 1 > size.Stock) {
                    return new MesAndStaDto("Insufficient stock to add this item", 400); // 400 for Bad Request
                }

                if (existingToBuyLater != null) {
                    // If it exists, increment the quantity
                    existingToBuyLater.Quantity = Math.Max(existingToBuyLater.Quantity + 1, 1); // Ensure quantity stays >= 1
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
                return new MesAndStaDto(
                    result > 0 ? "Saved successfully" : "No changes were made",
                    result > 0 ? 200 : 400 // 200 for success, 400 for no changes made
                );
            }
            catch (Exception ex) {
                // Log the exception and return a friendly error message
                Console.WriteLine($"Error: {ex.Message}");
                return new MesAndStaDto("An error occurred while saving the item. Please try again.", 500); // 500 for Internal Server Error
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


        public async Task<MesAndStaDto> UpdateQuantityInBuyLater (int sizeId, string userId, int quantity) {
            try {
                // Validate that quantity is positive
                if (quantity <= 0) {
                    return new MesAndStaDto("Quantity must be greater than zero", 400);
                }

                // Check if the size exists and fetch its details
                var size = await _context.Sizes.FirstOrDefaultAsync(s => s.SizeId == sizeId);
                if (size == null) {
                    return new MesAndStaDto("Size does not exist", 404); // 404 for Not Found
                }

                // Check if the stock is sufficient
                if (size.Stock < quantity) {
                    return new MesAndStaDto("Insufficient stock for the requested quantity", 400); // 400 for Bad Request
                }

                // Find the existing ToBuyLater item for the user and size
                var existingToBuyLater = await _context.ToBuyLaters
                    .FirstOrDefaultAsync(t => t.UserId == userId && t.SizeId == sizeId);

                if (existingToBuyLater != null) {
                    // Update the quantity
                    existingToBuyLater.Quantity = quantity;

                    // Save the changes to the database
                    var result = await _context.SaveChangesAsync();

                    // Return success message if changes were saved
                    return new MesAndStaDto(
                       result > 0 ? "Quantity updated successfully" : "No changes were made",
                       result > 0 ? 200 : 400 // 200 for success, 400 for no changes made
                    );
                }
                else {
                    return new MesAndStaDto("Item does not exist in ToBuyLater list", 404); // 404 for Not Found
                }
            }
            catch (Exception ex) {
                // Log the exception and return a friendly error message
                Console.WriteLine($"Error: {ex.Message}");
                return new MesAndStaDto("An error occurred while updating the quantity. Please try again.", 500); // 500 for Internal Server Error
            }
        }




        public async Task<MesAndStaDto> UpdateAddQuantityInBuyLater (int sizeId, string userId, int quantity) {
            try {
                // Validate that quantity is positive
                if (quantity <= 0) {
                    return new MesAndStaDto("Quantity must be greater than zero", 400);
                }

                // Check if the size exists and fetch its details
                var size = await _context.Sizes.FirstOrDefaultAsync(s => s.SizeId == sizeId);
                if (size == null) {
                    return new MesAndStaDto("Size does not exist", 404); // 404 for Not Found
                }



                // Find the existing ToBuyLater item for the user and size
                var existingToBuyLater = await _context.ToBuyLaters
                    .FirstOrDefaultAsync(t => t.UserId == userId && t.SizeId == sizeId);


                // Check if the stock is sufficient
                if (size.Stock < existingToBuyLater?.Quantity + quantity) {
                    return new MesAndStaDto("Insufficient stock for the requested quantity", 400); // 400 for Bad Request
                }

                if (existingToBuyLater != null) {
                    // Update the quantity
                    existingToBuyLater.Quantity += quantity;

                    // Save the changes to the database
                    var result = await _context.SaveChangesAsync();

                    // Return success message if changes were saved
                    return new MesAndStaDto(
                       result > 0 ? "Quantity updated successfully" : "No changes were made",
                       result > 0 ? 200 : 400 // 200 for success, 400 for no changes made
                    );
                }
                else {
                    return new MesAndStaDto("Item does not exist in ToBuyLater list", 404); // 404 for Not Found
                }
            }
            catch (Exception ex) {
                // Log the exception and return a friendly error message
                Console.WriteLine($"Error: {ex.Message}");
                return new MesAndStaDto("An error occurred while updating the quantity. Please try again.", 500); // 500 for Internal Server Error
            }
        }

        public async Task<MesAndStaDto> RemoveFromBuyLater (int sizeId, string userId) {
            var size = await _context.Sizes.FirstOrDefaultAsync(s => s.SizeId == sizeId);
            if (size == null) {
                return new MesAndStaDto("Size does not exist", 404); // 404 for Not Found
            }

            var existingToBuyLater = await _context.ToBuyLaters
                .FirstOrDefaultAsync(t => t.UserId == userId && t.SizeId == sizeId);

            if (existingToBuyLater == null) {
                return new MesAndStaDto("Item not found in 'Buy Later' list", 404); // 404 for Not Found
            }

            _context.ToBuyLaters.Remove(existingToBuyLater);

            try {
                await _context.SaveChangesAsync();
                return new MesAndStaDto("Item removed from 'Buy Later' list successfully", 200); // 200 for OK
            }
            catch (Exception ex) {
                // Log the exception (if a logging system is in place)
                return new MesAndStaDto($"An error occurred while removing the item: {ex.Message}", 500); // 500 for Internal Server Error
            }
        }

    }

}

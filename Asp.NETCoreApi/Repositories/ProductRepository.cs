using Asp.NETCoreApi.Data;
using Asp.NETCoreApi.Dto;
using Asp.NETCoreApi.IRepositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Asp.NETCoreApi.Repositories {
    public class ProductRepository : IProductRepository {

        private readonly MyDbContext _context;
        private readonly IMapper _mapper;

        public ProductRepository (MyDbContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ProductDto> AddProduct (ProductDto productDto) {


            try {
                // Add the main product
                var product = new Product {
                    Name = productDto.Name,
                    Price = productDto.Price,
                    Discount = productDto.Discount,
                    ProductCategoryId = productDto.CategoryProductDtoId
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync(); // Save to get the ProductId

                // Add colors and their associated sizes and images
                foreach (var colorDto in productDto.ColorDtos) {
                    var color = new Color {
                        Name = colorDto.Name,
                        HexCode = colorDto.HexCode,
                        ProductId = product.ProductId
                    };

                    _context.Colors.Add(color);
                    await _context.SaveChangesAsync(); // Save to get the ColorId

                    // Add sizes for the color
                    foreach (var sizeDto in colorDto.SizeDtos) {
                        var size = new Size {
                            Name = sizeDto.Name,
                            Stock = sizeDto.Stock,
                            ColorId = color.ColorId
                        };

                        _context.Sizes.Add(size);
                    }

                    // Add images for the color
                    foreach (var imageDto in colorDto.ImageDtos) {
                        var image = new Image {
                            ColorId = color.ColorId,
                            Data = imageDto.Data // Directly assign byte[] data
                        };

                        _context.Images.Add(image);
                    }
                }

                // Save sizes and images in bulk
                await _context.SaveChangesAsync();

                // Return the complete ProductDto
                productDto.ProductDtoId = product.ProductId;
                return productDto;
            }
            catch (Exception ex) {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }



        public async Task<ProductDto> GetProductDtoById (int id) {
            var product = await _context.Products
                .Include(p => p.ProductCategory)
                .Include(p => p.Colors)
                    .ThenInclude(c => c.Sizes)
                .Include(p => p.Colors)
                    .ThenInclude(c => c.Images)
                .SingleOrDefaultAsync(p => p.ProductId == id);  // Make it asynchronous

            if (product == null) {
                return null;  // If product is not found, return null
            }

            var productDto = new ProductDto {
                ProductDtoId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                Discount = product.Discount,
                CategoryProductDtoId = product.ProductCategory.ProductCategoryId,  // Fixed property mapping
                ColorDtos = new List<ColorDto>()
            };

            if (product.Colors != null)  // Check if Colors is not null
            {
                foreach (var color in product.Colors) {
                    var colorDto = new ColorDto {
                        ColorDtoId = color.ColorId,
                        Name = color.Name,
                        HexCode = color.HexCode,
                        SizeDtos = new List<SizeDto>(),
                        ImageDtos = new List<ImageDto>()
                    };

                    if (color.Sizes != null)  // Check if Sizes is not null
                    {
                        foreach (var size in color.Sizes) {
                            colorDto.SizeDtos.Add(new SizeDto {
                                SizeDtoId = size.SizeId,
                                Name = size.Name,
                                Stock = size.Stock
                            });
                        }
                    }

                    if (color.Images != null)  // Check if Images is not null
                    {
                        foreach (var image in color.Images) {
                            colorDto.ImageDtos.Add(new ImageDto {
                                ImageDtoId = image.ImageId,
                                Data = image.Data
                            });
                        }
                    }

                    productDto.ColorDtos.Add(colorDto);
                }
            }

            return productDto;  // Return the DTO
        }


        public async Task<PaginatedDto<ProductDto>> GetProductsWithPagination (string name, int pageNumber, int pageSize) {
            // Truy vấn sản phẩm liên quan đến tên sản phẩm
            var query = _context.Products
                .Include(p => p.Colors)
                    .ThenInclude(c => c.Sizes)
                .Include(p => p.Colors)
                    .ThenInclude(c => c.Images)
                .Where(p => EF.Functions.Like(p.Name, $"%{name}%")); // Lọc theo tên sản phẩm

            // Tổng số lượng sản phẩm
            var totalItems = await query.CountAsync();

            // Lấy danh sách sản phẩm theo phân trang
            var products = await query
                .Skip(( pageNumber - 1 ) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Tạo danh sách ProductDto
            var productDtos = products
                .Select(product => new ProductDto {
                    ProductDtoId = product.ProductId,
                    Name = product.Name,
                    Price = product.Price,
                    Discount = product.Discount,
                    ColorDtos = product.Colors.Select(color => new ColorDto {
                        ColorDtoId = color.ColorId,
                        Name = color.Name,
                        HexCode = color.HexCode,
                        SizeDtos = color.Sizes.Select(size => new SizeDto {
                            SizeDtoId = size.SizeId,
                            Name = size.Name,
                            Stock = size.Stock
                        }).ToList(),
                        ImageDtos = color.Images.Select(image => new ImageDto {
                            ImageDtoId = image.ImageId,
                            Data = image.Data // Assuming `Data` is a Base64 string or byte array
                        }).ToList()
                    }).ToList()
                })
                .ToList();

            // Trả về dữ liệu dạng phân trang
            return new PaginatedDto<ProductDto> {
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = productDtos
            };
        }






    }

}

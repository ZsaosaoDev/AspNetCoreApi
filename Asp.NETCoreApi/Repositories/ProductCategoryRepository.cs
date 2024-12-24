using Asp.NETCoreApi.Data;
using Asp.NETCoreApi.Dto;
using Asp.NETCoreApi.IRepositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Asp.NETCoreApi.Repositories {
    public class ProductCategoryRepository : IProductCategoryRepository {

        private readonly MyDbContext _context;
        private readonly IMapper _mapper;

        public ProductCategoryRepository (MyDbContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }



        public async Task<List<ProductCategoryDto>> GetAllProductCategories () {
            // Lấy dữ liệu từ database
            var productCategories = await _context.ProductCategories.ToListAsync();

            // Map sang DTO bằng AutoMapper
            var result = _mapper.Map<List<ProductCategoryDto>>(productCategories);

            return result;
        }




        public async Task<ProductCategoryDto> AddProductCategory (ProductCategoryDto productCategoryDto) {

            ProductCategory productCategory = new ProductCategory {
                Name = productCategoryDto.Name,
                Description = productCategoryDto.Description,
                Picture = productCategoryDto.Picture
            };

            _context.ProductCategories.Add(productCategory);
            await _context.SaveChangesAsync(); // Save to get the ProductCategoryId

            return productCategoryDto;
        }





        public async Task<List<ProductCategoryDto>> GetProductCategories (string name) {
            // Fetch product categories with their related entities
            var productCategories = await _context.ProductCategories
                .Include(pc => pc.Products) // Include Products
                    .ThenInclude(p => p.Colors) // Products have Colors
                        .ThenInclude(c => c.Sizes) // Colors have Sizes
                .Include(pc => pc.Products) // Include Products again
                    .ThenInclude(p => p.Colors) // Products have Colors
                        .ThenInclude(c => c.Images) // Colors have Images
                .Where(pc => EF.Functions.Like(pc.Name, $"%{name}%")) // Filter by name
                .ToListAsync();

            // Map to ProductCategoryDto
            var productCategoryDtos = productCategories.Select(pc => new ProductCategoryDto {
                Name = pc.Name,
                Description = pc.Description,
                Id = pc.ProductCategoryId,
                ProductDtos = pc.Products.Select(p => new ProductDto {
                    ProductDtoId = p.ProductId,
                    Name = p.Name,
                    Price = p.Price,
                    Discount = p.Discount,
                    ColorDtos = p.Colors.Select(c => new ColorDto {
                        ColorDtoId = c.ColorId,
                        Name = c.Name,
                        HexCode = c.HexCode,
                        SizeDtos = c.Sizes.Select(s => new SizeDto {
                            SizeDtoId = s.SizeId,
                            Name = s.Name,
                            Stock = s.Stock
                        }).ToList(),
                        ImageDtos = c.Images.Select(i => new ImageDto {
                            ImageDtoId = i.ImageId,
                            Data = i.Data // Assuming this is a string or byte[] representing the image
                        }).ToList()
                    }).ToList()
                }).ToList()
            }).ToList();

            return productCategoryDtos;
        }

        public async Task<PaginatedDto<ProductDto>> GetProductsWithPagination (string categoryName, int pageNumber, int pageSize, string productName) {
            // Truy vấn sản phẩm liên quan đến danh mục
            var query = _context.Products
            .Include(p => p.Colors)
                .ThenInclude(c => c.Sizes)
            .Include(p => p.Colors)
                .ThenInclude(c => c.Images)
            .Where(p => EF.Functions.Like(p.ProductCategory.Name, $"%{categoryName}%")
                && EF.Functions.Like(p.Name, $"%{productName}%"));


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

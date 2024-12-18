using Asp.NETCoreApi.Dto;

namespace Asp.NETCoreApi.IRepositories {
    public interface IProductRepository {

        Task<ProductDto> AddProduct (ProductDto productDto);
        Task<ProductDto> GetProductDtoById (int id);

        Task<PaginatedDto<ProductDto>> GetProductsWithPagination (string productName, int pageNumber, int pageSize);



    }
}

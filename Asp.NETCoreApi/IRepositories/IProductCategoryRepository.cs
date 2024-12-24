using Asp.NETCoreApi.Dto;

namespace Asp.NETCoreApi.IRepositories {
    public interface IProductCategoryRepository {

        //int GetIdByName (string name);

        Task<List<ProductCategoryDto>> GetAllProductCategories ();

        Task<ProductCategoryDto> AddProductCategory (ProductCategoryDto productCategoryDto);

        Task<List<ProductCategoryDto>> GetProductCategories (string name);

        Task<PaginatedDto<ProductDto>> GetProductsWithPagination (string name, int pageNumber, int pageSize, string productName);

    }
}

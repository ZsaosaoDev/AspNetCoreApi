using Asp.NETCoreApi.Dto;

namespace Asp.NETCoreApi.IRepositories {
    public interface IToBuyLaterRepository {

        Task<string> SaveToBuyLater (int sizeId, string userId);

        Task<List<ProductDto>> GetProductsWithSelectedColors (string userId);
    }
}

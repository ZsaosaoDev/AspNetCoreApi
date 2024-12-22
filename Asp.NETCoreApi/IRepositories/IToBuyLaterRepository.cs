using Asp.NETCoreApi.Dto;

namespace Asp.NETCoreApi.IRepositories {
    public interface IToBuyLaterRepository {

        Task<MesAndStaDto> SaveToBuyLater (int sizeId, string userId);

        Task<List<ProductDto>> GetProductsWithSelectedColors (string userId);

        Task<MesAndStaDto> UpdateQuantityInBuyLater (int sizeId, string userId, int quantity);
        Task<MesAndStaDto> UpdateAddQuantityInBuyLater (int sizeId, string userId, int quantity);

    }
}

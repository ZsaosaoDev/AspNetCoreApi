using Asp.NETCoreApi.Dto;

namespace Asp.NETCoreApi.IRepositories {
    public interface IOrderHistory {

        Task<List<HistoryOrderDto>> GetOrdersByUserId (string userId);

        Task<List<ProductDto>> GetProductsByOrderIdAsync (int orderId, string userId);
    }
}

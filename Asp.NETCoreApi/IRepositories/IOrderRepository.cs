using Asp.NETCoreApi.Dto;

namespace Asp.NETCoreApi.IRepositories {
    public interface IOrderRepository {

        Task<MesAndStaDto> Order (List<int> sizeIds, string userId, int paymentId, int deliverId);
    }
}

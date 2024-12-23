using Asp.NETCoreApi.Dto;

namespace Asp.NETCoreApi.IRepositories {
    public interface IDeliveryRepository {
        Task<MesAndStaDto> AddDeliveryAsync (DeliveryDto paymentDto);

        Task<List<DeliveryDto>> GetAllDeliveryAsync ();
    }
}

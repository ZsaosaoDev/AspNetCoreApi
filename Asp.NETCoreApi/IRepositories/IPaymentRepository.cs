using Asp.NETCoreApi.Dto;

namespace Asp.NETCoreApi.IRepositories {
    public interface IPaymentRepository {
        Task<MesAndStaDto> AddPaymentAsync (PaymentDto paymentDto);

        Task<List<PaymentDto>> GetAllPaymentsAsync ();

    }
}

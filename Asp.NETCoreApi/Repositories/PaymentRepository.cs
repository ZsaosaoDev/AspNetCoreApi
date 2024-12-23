using Asp.NETCoreApi.Data;
using Asp.NETCoreApi.Dto;
using Asp.NETCoreApi.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Asp.NETCoreApi.Repositories {
    public class PaymentRepository : IPaymentRepository {

        private readonly MyDbContext _context;

        public PaymentRepository (MyDbContext context) {
            _context = context;
        }


        public async Task<MesAndStaDto> AddPaymentAsync (PaymentDto paymentDto) {
            try {
                // Tạo đối tượng Payment từ PaymentDto
                var payment = new Payment {
                    MeThodPayment = paymentDto.MeThodPayment
                };

                // Thêm vào cơ sở dữ liệu
                await _context.Payments.AddAsync(payment);
                await _context.SaveChangesAsync();

                // Trả về kết quả thành công
                return new MesAndStaDto("Payment added successfully", 200); // Mã 200 là thành công
            }
            catch (Exception ex) {
                return new MesAndStaDto($"Failed to add payment: {ex.Message}", 500); // Mã 500 là lỗi server
            }



        }
        public async Task<List<PaymentDto>> GetAllPaymentsAsync () {
            try {
                // Lấy tất cả các Payment từ cơ sở dữ liệu
                var payments = await _context.Payments
                    .Select(p => new PaymentDto {
                        PaymentId = p.PaymentId,
                        MeThodPayment = p.MeThodPayment
                    })
                    .ToListAsync();

                // Trả về danh sách PaymentDto
                return payments;
            }
            catch (Exception ex) {

                return new List<PaymentDto>();
            }
        }

    }

}


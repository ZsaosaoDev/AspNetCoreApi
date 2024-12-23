using Asp.NETCoreApi.Data;
using Asp.NETCoreApi.Dto;
using Asp.NETCoreApi.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Asp.NETCoreApi.Repositories {
    public class DeliveryRepository : IDeliveryRepository {
        private readonly MyDbContext _context;

        public DeliveryRepository (MyDbContext context) {
            _context = context;
        }

        // Phương thức thêm Delivery
        public async Task<MesAndStaDto> AddDeliveryAsync (DeliveryDto deliveryDto) {
            try {
                // Tạo đối tượng Delivery từ DeliveryDto
                var delivery = new Deliver {
                    DeliverType = deliveryDto.DeliveryType,
                    Fee = deliveryDto.Fee
                };

                // Thêm vào cơ sở dữ liệu
                await _context.Delivers.AddAsync(delivery);
                await _context.SaveChangesAsync();

                // Trả về kết quả thành công
                return new MesAndStaDto("Delivery added successfully", 200); // Mã 200 là thành công
            }
            catch (Exception ex) {

                // Trả về kết quả lỗi
                return new MesAndStaDto($"Failed to add delivery: {ex.Message}", 500); // Mã 500 là lỗi server
            }
        }

        // Phương thức lấy tất cả Delivery
        public async Task<List<DeliveryDto>> GetAllDeliveryAsync () {
            try {
                // Lấy tất cả các Delivery từ cơ sở dữ liệu
                var deliveries = await _context.Delivers
                    .Select(d => new DeliveryDto {
                        DeliveryId = d.DeliverId,
                        DeliveryType = d.DeliverType,
                        Fee = d.Fee
                    })
                    .ToListAsync();

                // Trả về danh sách DeliveryDto
                return deliveries;
            }
            catch (Exception ex) {

                return new List<DeliveryDto>();
            }
        }


    }
}

using System.ComponentModel.DataAnnotations;

namespace Asp.NETCoreApi.Data {
    public class OrderDetail {
        [Key]
        public int OrderDetailId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public int SizeId { get; set; }

        public Size Size { get; set; }
        public int OrderId { get; set; }



        public Order Order { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Asp.NETCoreApi.Data {
    public class Order {
        [Key]
        public int OrderId { get; set; }
        public string OrderDate { get; set; }

        public string OrderStatus { get; set; }

        public string Address { get; set; }

        public decimal TotalPrice { get; set; }

        public string UserId { get; set; }



        public int PaymentId { get; set; }
        public Payment Payment { get; set; }

        public int DeliverId { get; set; }
        public Deliver Delivery { get; set; }

        List<OrderDetail> OrderDetails { get; set; }

    }
}
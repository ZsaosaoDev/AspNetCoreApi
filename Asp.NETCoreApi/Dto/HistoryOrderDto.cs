namespace Asp.NETCoreApi.Dto {
    public class HistoryOrderDto {

        public int OrderId { get; set; }
        public string OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; }  // Tên phương thức thanh toán
        public string DeliverMethod { get; set; }  // Tên phương thức giao hàng
    }
}

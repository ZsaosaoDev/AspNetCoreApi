namespace Asp.NETCoreApi.Dto {
    public class OrderRequestDto {

        public List<int> SizeIds { get; set; }
        public int PaymentId { get; set; }
        public int DeliverId { get; set; }
    }
}

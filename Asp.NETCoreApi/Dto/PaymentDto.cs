namespace Asp.NETCoreApi.Dto {
    public class PaymentDto {

        public string MeThodPayment { get; set; }
        public int PaymentId { get; set; }

        public PaymentDto () { }

        public PaymentDto (string meThodPayment) {
            MeThodPayment = meThodPayment;
        }
    }
}

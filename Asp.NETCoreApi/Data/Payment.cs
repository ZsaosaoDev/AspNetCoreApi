using System.ComponentModel.DataAnnotations;

namespace Asp.NETCoreApi.Data {
    public class Payment {
        [Key]
        public int PaymentId { get; set; }

        public string MeThodPayment { get; set; }

        public List<Order> Orders { get; set; }
    }
}

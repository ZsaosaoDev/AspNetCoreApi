using System.ComponentModel.DataAnnotations;

namespace Asp.NETCoreApi.Data {
    public class Deliver {
        [Key]
        public int DeliverId { get; set; }
        public string DeliverType { get; set; }

        public float Fee { get; set; }

        public List<Order> Orders { get; set; }
    }
}

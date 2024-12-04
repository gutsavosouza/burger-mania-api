using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace burger_mania_api.Entities
{
    public class Order
    {
        [Key]
        public int Id {get; set;}

        [Required]
        public required int StatusId {get; set;}

        [ForeignKey(nameof(StatusId))]    
        [JsonIgnore]
        public Status? Status { get; set;} 

        [NotMapped]
        public float TotalPrice 
        {
            get
            {
                return OrdersProducts?.Sum(op => (op.Product?.Price ?? 0) * op.Quantity) ?? 0;
            }
        }

        public ICollection<OrdersProducts> OrdersProducts { get; set; } = new List<OrdersProducts>();

        public List<UsersOrders>? UsersOrders { get; set; }
    }
}
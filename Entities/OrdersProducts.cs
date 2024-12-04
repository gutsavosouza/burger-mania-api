using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace burger_mania_api.Entities
{
    public class OrdersProducts
    {
        [Key]
        public int Id {get; set; }

        [Required]
        public required int ProductId {get; set; }
        
        [Required]
        public required int OrderId {get; set; }

        public int Quantity { get; set; }

        [ForeignKey(nameof(ProductId))]    
        [JsonIgnore]
        public Product? Product { get; set; } 

        [ForeignKey(nameof(OrderId))]    
        [JsonIgnore]
        public Order? Order { get; set; } 
    }
}
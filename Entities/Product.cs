using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace burger_mania_api.Entities
{
    public class Product
    {
        [Key]
        public int Id {get; set;} 

        [StringLength(127)]
        [Required]
        public required string Name {get; set;}

        [Required]
        [StringLength(255)]
        public required string ImagePath {get; set;}

        [Required]
        public required float Price {get; set;}

        [StringLength(255)]
        [Required]
        public required string Ingredients {get; set;}

        [StringLength(511)]
        [Required]
        public required string Description {get; set;}

        public int? CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]    
        [JsonIgnore]
        public Category? Category { get; set; } 

        public List<OrdersProducts>? OrdersProducts { get; set; }
    }   
}
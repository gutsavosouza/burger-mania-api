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

        [Required]
        public required float TotalPrice {get; set;}
    }
}
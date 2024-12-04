using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace burger_mania_api.Entities
{
    public class UsersOrders
    {
        [Key]
        public int Id {get; set;}

        [Required]
        public required int UserId {get; set;}
        
        [Required]
        public required int OrderId {get; set;}

        [ForeignKey(nameof(UserId))]    
        [JsonIgnore]
        public User? User { get; set;} 

        [ForeignKey(nameof(OrderId))]    
        [JsonIgnore]
        public Order? Order { get; set;} 
    }
}
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace burger_mania_api.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class Status
    {
        [Key]
        public int Id {get; set;}

        [Required]
        [StringLength(127)]
        public required string Name {get; set;}

        public ICollection<Order>? Orders { get; set; }
    }
}
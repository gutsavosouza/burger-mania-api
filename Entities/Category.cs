using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace burger_mania_api.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class Category
    {
        [Key]
        public int Id {get; set;}

        [Required]
        [StringLength(127)]
        public required string Name {get; set;}

        [Required]
        [StringLength(255)]
        public required string Description {get; set;}

        [Required]
        [StringLength(255)]
        public required string ImagePath {get; set;}

        public ICollection<Product>? Products { get; set; }
    }
}
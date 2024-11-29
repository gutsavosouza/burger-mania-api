using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace burger_mania_api.Entities
{
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        [Key]
        public int Id {get; set;}

        [Required]
        [StringLength(255)]
        public required string Name {get; set;}

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public required string Email {get; set;}

        [Required]
        [StringLength(255)]
        public required string Password {get; set;}
    }
}
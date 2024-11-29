namespace burger_mania_api.DTOs
{
    public class ProductDTO
    {
        public required string Name {get; set;}
        public required string ImagePath {get; set;}
        public required float Price {get; set;}
        public required string Ingredients {get; set;}
        public required string Description {get; set;}
        public int? CategoryId { get; set; }
    }   
}
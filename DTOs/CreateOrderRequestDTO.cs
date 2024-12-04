namespace burger_mania_api.DTOs
{
    public class CreateOrderRequestDTO
    {
        public required  int UserId { get; set; }
        public required List<ProductQuantityDTO> Products { get; set; }
    }

    public class ProductQuantityDTO
    {
        public required int ProductId { get; set; }
        public required int Quantity { get; set; }
    }

}

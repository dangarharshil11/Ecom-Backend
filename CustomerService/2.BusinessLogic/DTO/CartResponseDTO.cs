namespace CustomerService._2.BusinessLogic.DTO
{
    public class CartResponseDTO
    {
        public required Guid Id { get; set; }
        public required Guid UserId { get; set; }
        public UserDTO? User { get; set; }
        public required Guid ProductId { get; set; }
        public ProductDTO? Product { get; set; }
        public required int ProductCount { get; set; }
    }
}

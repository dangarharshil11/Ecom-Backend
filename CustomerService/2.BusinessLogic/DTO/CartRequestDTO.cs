namespace CustomerService._2.BusinessLogic.DTO
{
    public class CartRequestDTO
    {
        public required Guid UserId { get; set; }
        public required Guid ProductId { get; set; }
        public required int ProductCount { get; set; }
    }
}

namespace CustomerService._2.BusinessLogic.DTO
{
    public class UpdateOrderRequestDTO
    {
        public required Guid OrderId { get; set; }
        public required string Status { get; set; }
        public string? Note { get; set; }
    }
}

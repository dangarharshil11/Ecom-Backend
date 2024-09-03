namespace CustomerService._2.BusinessLogic.DTO
{
    public class OrderDTO
    {
        public Guid? Id { get; set; }
        public required Guid UserId { get; set; }
        public UserDTO? User { get; set; }
        public required double OrderAmount { get; set; }
        public string? Status { get; set; }
        public string? Note { get; set; }
        public required DateTime OrderTime { get; set; }
        public IEnumerable<OrderDetailsDTO> OrderDetails { get; set; }
    }
}

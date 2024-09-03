namespace CustomerService._2.BusinessLogic.DTO
{
    public class OrderDetailsDTO
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public required Guid ProductId { get; set; }
        public ProductDTO? Product { get; set; }
        public required double ProductCount { get; set; }
        public required double ProductUnitPrice { get; set; }
    }
}

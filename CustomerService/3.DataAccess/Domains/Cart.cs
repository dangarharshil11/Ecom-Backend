namespace CustomerService._3.DataAccess.Domains
{
    public class Cart
    {
        public required Guid Id { get; set; }
        public required Guid UserId { get; set; }
        public required Guid ProductId { get; set; }
        public required int ProductCount { get; set; }
    }
}

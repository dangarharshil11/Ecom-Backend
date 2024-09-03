namespace CustomerService._3.DataAccess.Domains
{
    public class Order
    {
        public required Guid Id { get; set; }
        public required Guid UserId { get; set; }
        public required double OrderAmount { get; set; }
        public required DateTime OrderTime { get; set; }
        public string? Status { get; set; }
        public string? Note { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? StripeSessionId { get; set; }
        public IEnumerable<OrderDetail> OrderDetails { get; set; }
    }
}

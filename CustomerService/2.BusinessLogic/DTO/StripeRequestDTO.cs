namespace CustomerService._2.BusinessLogic.DTO
{
    public class StripeRequestDTO
    {
        public string? StripeSessionUrl { get; set; }
        public string? StripeSessionId { get; set; }
        public required string ApprovedUrl { get; set; }
        public required string CancelUrl { get; set; }
        public OrderDTO? Order { get; set; }
    }
}

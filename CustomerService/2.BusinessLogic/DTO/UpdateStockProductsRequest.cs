namespace CustomerService._2.BusinessLogic.DTO
{
    public class UpdateStockProductsRequest
    {
        public required List<Guid> ProductIds { get; set; }
        public required List<int> Counts { get; set; }
    }
}

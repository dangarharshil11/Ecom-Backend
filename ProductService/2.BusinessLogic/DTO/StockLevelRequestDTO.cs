namespace ProductService._2.BusinessLogic.DTO
{
    public class StockLevelRequestDTO
    {
        public Guid ProductId { get; set; }
        public required int CurrentStockItems { get; set; }
        public required int ThresholdAmount { get; set; }
        public required string SupplierName { get; set; }
        public required string SupplierEmail { get; set; }
        public DateTime? LastOrderDate { get; set; }
    }
}

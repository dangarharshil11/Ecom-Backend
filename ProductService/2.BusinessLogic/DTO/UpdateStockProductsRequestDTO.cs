namespace ProductService._2.BusinessLogic.DTO
{
    public class UpdateStockProductsRequestDTO
    {
        public required List<Guid> ProductIds { get; set; }
        public required List<int> Counts { get; set; }
    }
}

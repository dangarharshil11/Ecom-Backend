namespace CustomerService._2.BusinessLogic.DTO
{
    public class ProductDTO
    {
        public required Guid Id { get; set; }
        public required string ProductName { get; set; }
        public required string ProductDescription { get; set; }
        public required Guid CategoryId { get; set; }
        public CategoryDTO? Category { get; set; }
        public required double ProductPrice { get; set; }
        public required string ProductBrandName { get; set; }
        public required string ImageRepresentationBase64 { get; set; }
        public required DateTime CreatedDate { get; set; }
        public int? StockItems { get; set; }
    }
}

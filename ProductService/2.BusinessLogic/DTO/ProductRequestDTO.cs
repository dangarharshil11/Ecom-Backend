namespace ProductService._2.BusinessLogic.DTO
{
    public class ProductRequestDTO
    {
        public required string ProductName { get; set; }
        public required string ProductDescription { get; set; }
        public required double ProductPrice { get; set; }
        public required Guid CategoryId { get; set; }
        public required string ProductBrandName { get; set; }
        public required string ImageRepresentationBase64 { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}

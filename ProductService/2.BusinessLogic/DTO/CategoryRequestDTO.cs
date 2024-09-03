namespace ProductService._2.BusinessLogic.DTO
{
    public class CategoryRequestDTO
    {
        public required string CategoryName { get; set; }
        public required string CategoryDescription { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}

namespace CustomerService._2.BusinessLogic.DTO
{
    public class CategoryDTO
    {
        public required Guid Id { get; set; }
        public required string CategoryName { get; set; }
        public required string CategoryDescription { get; set; }
        public required DateTime CreatedDate { get; set; }
    }
}

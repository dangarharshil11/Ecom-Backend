namespace ProductService._3.DataAccess.Domains
{
    public class Category
    {
        public required Guid Id { get; set; }
        public required string CategoryName { get; set; }
        public required string CategoryDescription { get; set; }
        public required DateTime CreatedDate { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductService._3.DataAccess.Domains
{
    public class Product
    {
        public required Guid Id { get; set; }
        public required string ProductName { get; set; }
        public required string ProductDescription { get; set; }
        public required Guid CategoryId { get; set;}
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category? Category { get; set; }
        public required double ProductPrice { get; set; }
        public required string ProductBrandName { get; set; }
        public required string ImageRepresentationBase64 { get; set; }
        public required DateTime CreatedDate { get; set; }
        [NotMapped]
        public int? StockItems { get; set; }
    }
}

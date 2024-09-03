using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductService._3.DataAccess.Domains
{
    public class StockLevel
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product? Product { get; set; }
        public required int CurrentStockItems { get; set; }
        public required int ThresholdAmount { get; set; }
        public required string SupplierName { get; set; }
        public required string SupplierEmail { get; set; }
        public DateTime? LastOrderDate { get; set; }
        public DateTime? LastEmailSentDate { get; set; }
    }
}

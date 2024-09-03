using CustomerService._2.BusinessLogic.DTO;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerService._3.DataAccess.Domains
{
    public class OrderDetail
    {
        public required Guid Id { get; set; }
        public required Guid OrderId { get; set; }
        [ForeignKey("OrderId")]
        [ValidateNever]
        public Order? Order { get; set; }
        public required Guid ProductId { get; set; }
        [NotMapped]
        public ProductDTO? Product { get; set; }
        public required double ProductCount { get; set; }
        public required double ProductUnitPrice { get; set; }
    }
}

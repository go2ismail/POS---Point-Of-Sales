using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace core22.Models.POS.Models
{
    public class SalesOrder
    {
        public Guid SalesOrderId { get; set; }
        [Required]
        public string Number { get; set; }
        public string Description { get; set; }
        public DateTimeOffset? SalesOrderDate { get; set; } = DateTime.Now;
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public virtual List<SalesOrderLine> SalesOrderLine { get; set; } = new List<SalesOrderLine>();
    }

    public class SalesOrderLine
    {
        public Guid SalesOrderLineId { get; set; }
        public Guid SalesOrderId { get; set; }
        [JsonIgnore]
        public SalesOrder SalesOrder { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
    }
}

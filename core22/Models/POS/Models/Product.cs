using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace core22.Models.POS.Models
{
    public class Product
    {
        public Guid ProductId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public decimal PriceSell { get; set; }
        [Required]
        public decimal PricePurchase { get; set; }
    }
}

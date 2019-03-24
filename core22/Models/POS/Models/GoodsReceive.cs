using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace core22.Models.POS.Models
{
    public class GoodsReceive
    {
        public Guid GoodsReceiveId { get; set; }
        [Required]
        public string Number { get; set; }
        public string Description { get; set; }
        public DateTimeOffset? GoodsReceiveDate { get; set; } = DateTime.Now;
        public Guid PurchaseOrderId { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }
    }

    public class GoodsReceiveLine
    {
        public Guid GoodsReceiveLineId { get; set; }
        public Guid GoodsReceiveId { get; set; }
        [JsonIgnore]
        public GoodsReceive GoodsReceive { get; set; }
        public Guid PurchaseOrderLineId { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public int QtyPurchase { get; set; }
        public int QtyReceive { get; set; }
        public int QtyReceived { get; set; }
    }
}

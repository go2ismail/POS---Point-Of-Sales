using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using core22.Models.POS.Models;

namespace core22.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<core22.Models.POS.Models.Customer> Customer { get; set; }
        public DbSet<core22.Models.POS.Models.Vendor> Vendor { get; set; }
        public DbSet<core22.Models.POS.Models.Product> Product { get; set; }
        public DbSet<core22.Models.POS.Models.PurchaseOrder> PurchaseOrder { get; set; }
        public DbSet<core22.Models.POS.Models.SalesOrder> SalesOrder { get; set; }
        public DbSet<core22.Models.POS.Models.GoodsReceive> GoodsReceive { get; set; }
        public DbSet<core22.Models.POS.Models.InvenTran> InvenTran { get; set; }
        public DbSet<core22.Models.POS.Models.PurchaseOrderLine> PurchaseOrderLine { get; set; }
        public DbSet<core22.Models.POS.Models.SalesOrderLine> SalesOrderLine { get; set; }
        public DbSet<core22.Models.POS.Models.GoodsReceiveLine> GoodsReceiveLine { get; set; }
    }
}

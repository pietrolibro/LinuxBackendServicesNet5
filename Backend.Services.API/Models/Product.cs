using System;
using System.Collections.Generic;


namespace MyShopOnLine.Backend.Models
{
    public class Product
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
        public decimal Price { get; set; }

        public int Review { get; set; }
        public decimal Weight { get; set; }
        public int QuantityPerUnitPack { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }

    public class Order
    {
        public decimal Total { get; set; }
        public bool Shipped { get; set; }
        public bool Delivered { get; set; }
        public bool ReadyForShipping { get; set; }

        // Model building for fields, just to see another new, but be 
        // since fields are rarely public.
        public decimal Weight;

        public string Number { get; set; }
        public DateTime OrderDate { get; set; }
        public virtual Customer Customer { get; set; }

        public DateTime? ShippingDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public virtual ICollection<Product> Products { get; set; }

        public Order()
        {
            this.OrderDate = DateTime.UtcNow;
            this.Delivered = false;
            this.Shipped = false;
            this.ReadyForShipping = false;
            this.Products = new List<Product>();
        }
    }
}

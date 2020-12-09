namespace MyShopOnLine.Backend.Models
{

    public class OrderProduct
    {
        public Order Order { get; set; }
        public Product Product { get; set; }

        public string OrderNumber { get; set; }
        public string ProductCode { get; set; }

        public int Quantity { get; set; }
    }
}

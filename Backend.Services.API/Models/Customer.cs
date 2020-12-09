using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyShopOnLine.Backend.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public Address BillingAddress { get; set; }
        public Address ShippingAddress { get; set; }
        public bool ShippingAddressEqualsToBillingAddress { get; set; }

        public Customer()
        {
            this.ShippingAddressEqualsToBillingAddress = true;
        }
    }

    public class Address
    {
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Note { get; set; }
    }
}

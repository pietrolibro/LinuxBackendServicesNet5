using System;
using System.Collections.Generic;

namespace MyShopOnLine.Backend.Records
{
    public record OrderEntry(string ProductCode, int EntryQuantity = 0, decimal EntryPrice = 0, decimal EntryWeight = 0, string Description = "", int Review = 0);

    public record OrderRecord(string CustomerEmail,
         List<OrderEntry> OrderEntries, string Number = "", DateTime? OrderDate = null,
         decimal Total = 0, decimal Weight = 0, bool ReadyForShipping = false,
         bool Delivered = false, DateTime? DeliveryDate = null,
         bool Shipped = false, DateTime? ShppingDate = null);

    public record CustomerRecord(string Email, string Fullname, AddressRecord BillingAddress=null, AddressRecord ShippingAddress=null,
        bool ShippingAddressEqualsToBillingAddress = true);

    public record AddressRecord(string Street, string ZipCode, string City, string Note);
}

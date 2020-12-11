using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MyShopOnLine.Backend.Data;
using MyShopOnLine.Backend.Models;
using MyShopOnLine.Backend.Records;
using MyShopOnLine.Backend.Services.Results;

namespace MyShopOnLine.Backend.Services
{
    public static class OrderServiceExtensions
    {


        internal static OrderRecord GetRecord(this Order order, MyShopOnLineDataContext dbContext)
        {
            if (order == null) return null;

            var orderEntries = (from orderEntry in dbContext.OrderProducts
                                where orderEntry.OrderNumber == order.Number
                                select new { orderEntry.Product, orderEntry.Quantity })
                                .Select(oe => new OrderEntry(oe.Product.Code, oe.Quantity, oe.Product.Price,
                                oe.Product.Weight, oe.Product.Description, oe.Product.Review))
                                .ToList();

            return new OrderRecord(order.Customer.Email, orderEntries,
                order.Number, order.OrderDate, order.Total, order.Weight,
                order.ReadyForShipping, order.Delivered, order.DeliveryDate,
                order.Shipped, order.ShippingDate);
        }

        internal static async Task<Order> GetEntity(this OrderRecord record, MyShopOnLineDataContext dbContext)
        {
            if (record == null) return null;

            Customer customer = dbContext.Customers.FirstOrDefault(c => c.Email == record.CustomerEmail);

            if (customer == null) throw new Exception("Specified customer is not valid.");

            List<string> entryProductCodes = record.OrderEntries.Select(p => p.ProductCode).ToList();

            Dictionary<string, Product> products = await (from prod in dbContext.Products
                                                          where entryProductCodes.Contains(prod.Code)
                                                          select prod).ToDictionaryAsync(x => x.Code, x => x);

            Order order = new Order()
            {
                Customer = customer,
                DeliveryDate = record.DeliveryDate,
                Delivered = record.Delivered,
                Number = string.IsNullOrEmpty(record.Number) ? DateTime.UtcNow.ToString("yyyyMMddHHmmssfff") : record.Number,
                OrderDate = record.OrderDate ?? DateTime.UtcNow,
                ReadyForShipping = record.ReadyForShipping,
                Shipped = record.Shipped,
                ShippingDate = record.ShppingDate,
                Total = record.OrderEntries.Sum(e => products[e.ProductCode].Price * e.EntryQuantity),
                Weight = record.OrderEntries.Sum(e => products[e.ProductCode].Weight * e.EntryQuantity),
            };

            dbContext.OrderProducts.AddRange(record.OrderEntries.Select(oe => new OrderProduct()
            {
                Order = order,
                OrderNumber = order.Number,
                Product = products[oe.ProductCode],
                ProductCode = oe.ProductCode,
                Quantity = oe.EntryQuantity
            }).ToList());

            return order;
        }
    }

    public class OrderService : IOrderService
    {
        private readonly MyShopOnLineDataContext dbContext = null;

        public OrderService(MyShopOnLineDataContext context)
        {
            this.dbContext = context;

            context.SavingChanges += (sender, args) =>
            {
                Console.WriteLine($"Saving changes for {((DbContext)sender).Database.GetConnectionString()}");
            };

            context.SavedChanges += (sender, args) =>
            {
                Console.WriteLine($"Saved {args.EntitiesSavedCount} changes for {((DbContext)sender).Database.GetConnectionString()}");
            };
        }

        public async Task<List<OrderRecord>> GetAsync()
        {
            return (await this.dbContext.Orders.Include(o => o.Customer).ToListAsync())
                .Select(p => p.GetRecord(this.dbContext))
                .ToList();
        }

        public async Task<OrderRecord> GetAsync(string number)
        {
            Order order = await this.dbContext.Orders.FindAsync(number);

            return order.GetRecord(this.dbContext);
        }

        public async Task<CreateResult<OrderRecord>> CreateAsync(OrderRecord order)
        {
            if (OrderExists(order.Number))
            {
                return new CreateResult<OrderRecord>() { AlreadyExists = true, ErrorMessage = $"Order with number {order.Number} already exists." };
            }

            Order newOrder = await order.GetEntity(this.dbContext);

            try
            {
                await this.dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException dbu)
            {
                if (OrderExists(order.Number))
                {
                    return new CreateResult<OrderRecord>() { AlreadyExists = true };
                }
                else
                {
                    return new CreateResult<OrderRecord>() { ErrorMessage = dbu.Message };
                }
            }

            return new CreateResult<OrderRecord>() { Success = true, NewRecord = newOrder.GetRecord(this.dbContext) };
        }

        public async Task<UpdateResult> UpdateAsync(string number, OrderRecord orderIn)
        {
            if (!OrderExists(number))
            {
                return new UpdateResult() { NotFound = true, ErrorMessage = $"Product with code {orderIn.Number} doesn't exist." };
            }

            var orderTransaction = await this.dbContext.Database.BeginTransactionAsync();

            try
            {
                Order orderToDelete = await this.dbContext.Orders.FindAsync(orderIn.Number);

                this.dbContext.Remove(orderToDelete);

                await this.dbContext.SaveChangesAsync();

                await this.dbContext.AddAsync(await orderIn.GetEntity(dbContext));

                await this.dbContext.SaveChangesAsync();

                await orderTransaction.CommitAsync();
            }
            catch (DbUpdateConcurrencyException dce)
            {
                await orderTransaction.RollbackAsync();

                return new UpdateResult() { Success = false, ErrorMessage = dce.Message };
            }
            catch (Exception ex)
            {
                await orderTransaction.RollbackAsync();
            }

            return new UpdateResult() { Success = true };
        }

        public async Task<RemoveResult> RemoveAsync(string number)
        {
            Order toRemove = await this.dbContext.Orders.FindAsync(number);

            if ((toRemove) == null) return new RemoveResult() { NotFound = true };

            this.dbContext.Orders.Remove(toRemove);

            await this.dbContext.SaveChangesAsync();

            return new RemoveResult() { Success = true };
        }

        private bool OrderExists(string number)
        {
            return this.dbContext.Orders.Any(e => e.Number == number);
        }
    }
}

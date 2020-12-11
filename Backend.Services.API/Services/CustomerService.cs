
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyShopOnLine.Backend.Data;
using MyShopOnLine.Backend.Models;
using MyShopOnLine.Backend.Records;
using MyShopOnLine.Backend.Services.Results;

namespace MyShopOnLine.Backend.Services
{
    public static class CustomerServiceExtensions
    {
        internal static AddressRecord GetRecord(this Address address)
        {
            return address == null ? null : new AddressRecord(address.Street, address.ZipCode, address.City, address.Note);
        }

        internal static Address GetEntity(this AddressRecord address)
        {
            return address == null ? null : new Address()
            {
                City = address.City,
                Note = address.Note,
                Street = address.Street,
                ZipCode = address.ZipCode
            };
        }

        internal static CustomerRecord GetRecord(this Customer customer)
        {
            return customer == null ? null :
             new CustomerRecord(customer.Email, customer.Fullname,
                customer.BillingAddress.GetRecord(),
                customer.ShippingAddress.GetRecord(),
                customer.ShippingAddressEqualsToBillingAddress);
        }

        internal static Customer GetEntity(this CustomerRecord customer)
        {
            return customer == null ? null :
             new Customer()
             {
                 Email = customer.Email,
                 Fullname = customer.Fullname,
                 BillingAddress = customer.BillingAddress.GetEntity(),
                 ShippingAddress = customer.ShippingAddress.GetEntity()
             };
        }

        internal static Address GetRecord(this AddressRecord address)
        {
            if (address == null) return null;

            return new Address()
            {
                City = address.City,
                Note = address.Note,
                Street = address.Street,
                ZipCode = address.ZipCode
            };
        }
    }

    public class CustomerService : ICustomerService
    {
        private readonly MyShopOnLineDataContext dbContext = null;

        public CustomerService(MyShopOnLineDataContext context)
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

        public async Task<List<CustomerRecord>> GetAsync()
        {
            return (await this.dbContext.Customers.ToListAsync())
                .Select(p => p.GetRecord())
                .ToList();
        }

        public async Task<CustomerRecord> GetAsync(string email)
        {
            Customer customer = await this.dbContext.Customers.FirstOrDefaultAsync(c => c.Email.Equals(email));

            return customer.GetRecord();
        }

        public async Task<CreateResult<CustomerRecord>> CreateAsync(CustomerRecord customer)
        {
            if (CustomerExists(customer.Email))
            {
                return new CreateResult<CustomerRecord>() { AlreadyExists = true, ErrorMessage = $"Customer with email {customer.Email} already exists." };
            }

            Customer newCustomer = customer.GetEntity();

            this.dbContext.Customers.Add(newCustomer);

            try
            {
                await this.dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException dbu)
            {
                if (CustomerExists(customer.Email))
                {
                    return new CreateResult<CustomerRecord>() { AlreadyExists = true };
                }
                else
                {
                    return new CreateResult<CustomerRecord>() { ErrorMessage = dbu.Message };
                }
            }

            return new CreateResult<CustomerRecord>() { Success = true, NewRecord = newCustomer.GetRecord() };
        }

        public async Task<UpdateResult> UpdateAsync(string email, CustomerRecord customerIn)
        {
            if (!CustomerExists(email))
            {
                return new UpdateResult() { NotFound = true, ErrorMessage = $"Customer with email {customerIn.Email} doesn't exist." };
            }

            Customer customerToUpdate = await this.dbContext.Customers.FindAsync(email);

            customerToUpdate.Email = customerIn.Email;
            customerToUpdate.Fullname = customerIn.Fullname;
            customerToUpdate.BillingAddress = customerIn.BillingAddress.GetEntity();
            customerToUpdate.ShippingAddress = customerIn.ShippingAddress.GetEntity();

            this.dbContext.Entry(customerToUpdate).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            this.dbContext.Customers.Update(customerToUpdate);

            try
            {
                await this.dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException dce)
            {
                return new UpdateResult() { Success = false, ErrorMessage = dce.Message };
            }

            return new UpdateResult() { Success = true };
        }

        public async Task<RemoveResult> RemoveAsync(string email)
        {
            Customer toRemove = await this.dbContext.Customers.FirstOrDefaultAsync(c => c.Email.Equals(email));

            if ((toRemove) == null) return new RemoveResult() { NotFound = true };

            this.dbContext.Remove(toRemove);

            await this.dbContext.SaveChangesAsync();

            return new RemoveResult() { Success = true };
        }

        private bool CustomerExists(string email)
        {
            return this.dbContext.Customers.Any(e => e.Email == email);
        }
    }
}

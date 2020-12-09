
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;

using Microsoft.EntityFrameworkCore;

using MyShopOnLine.Backend.Data;
using MyShopOnLine.Backend.Models;
using MyShopOnLine.Backend.Records;
using MyShopOnLine.Backend.Services.Results;


namespace MyShopOnLine.Backend.Services
{

    public static class ProductServiceExtensions
    {
        internal static ProductRecord GetRecord(this Product product)
        {
            return product == null ? null :
             new ProductRecord(
                product.Code, product.Description, product.Cost,
                product.Price, product.Review, product.Weight,
                product.QuantityPerUnitPack);
        }

        internal static Product GetEntity(this ProductRecord record)
        {
            return record == null ? null :
             new Product()
             {
                 Code = record.Code,
                 Cost = record.Cost,
                 Description = record.Description,
                 Price = record.Price,
                 QuantityPerUnitPack = record.QuantityPerUnityPack,
                 Review = record.Review,
                 Weight = record.Weight
             };
        }
    }

    public class ProductService : IProductService
    {
        private readonly MyShopOnLineDataContext dbContext = null;

        public ProductService(MyShopOnLineDataContext context)
        {
            this.dbContext = context;
            this.dbContext.SavingChanges += DbContext_SavingChanges;
            this.dbContext.SavedChanges += DbContext_SavedChanges;
        }

        public async Task<List<ProductRecord>> GetAsync()
        {
            return (await this.dbContext.Products.ToListAsync())
                .Select(p => p.GetRecord())
                .ToList();
        }

        public async Task<ProductRecord> GetAsync(string code)
        {
            Product product = await this.dbContext.Products.FindAsync(code);

            return product.GetRecord();
        }

        public async Task<CreateResult<ProductRecord>> CreateAsync(ProductRecord product)
        {
            this.dbContext.Products.Add(product.GetEntity());

            try
            {
                await this.dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException dbu)
            {
                if (ProductExists(product.Code))
                {
                    return new CreateResult<ProductRecord>() { AlreadyExists = true };
                }
                else
                {
                    return new CreateResult<ProductRecord>() { ErrorMessage = dbu.Message };
                }
            }

            return new CreateResult<ProductRecord>() { Success = true, NewRecord = product };
        }

        public async Task<UpdateResult> UpdateAsync(string code, ProductRecord productIn)
        {
            if (!ProductExists(code))
            {
                return new UpdateResult() { NotFound = true, ErrorMessage = $"Product with code {productIn.Code} doesn't exist." };
            }

            Product productToUpdate = await this.dbContext.Products.FindAsync(code);

            productToUpdate.Cost = productIn.Cost;
            productToUpdate.Description = productIn.Description;
            productToUpdate.Price = productToUpdate.Price;
            productToUpdate.QuantityPerUnitPack = productToUpdate.QuantityPerUnitPack;
            productToUpdate.Review = productToUpdate.Review;
            productToUpdate.Weight = productToUpdate.Weight;

            this.dbContext.Entry(productToUpdate).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            this.dbContext.Products.Update(productToUpdate);

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

        public async Task<RemoveResult> RemoveAsync(string code)
        {
            Product toRemove = await this.dbContext.Products.FindAsync(code);

            if ((toRemove) == null) return new RemoveResult() { NotFound = true };

            this.dbContext.Remove(toRemove);

            await this.dbContext.SaveChangesAsync();

            return new RemoveResult() { Success = true };
        }

        private bool ProductExists(string code)
        {
            return this.dbContext.Products.Any(e => e.Code == code);
        }

        private void DbContext_SavingChanges(object sender, Microsoft.EntityFrameworkCore.SavingChangesEventArgs e)
        {
            Console.WriteLine("Saving...");
        }

        private void DbContext_SavedChanges(object sender, Microsoft.EntityFrameworkCore.SavedChangesEventArgs e)
        {
            Console.WriteLine("Saved...");
        }
    }
}

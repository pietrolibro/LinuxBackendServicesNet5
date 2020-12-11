using MyShopOnLine.Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Threading;

namespace MyShopOnLine.Backend.Data
{
    public class MyShopOnLineDataContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public MyShopOnLineDataContext(DbContextOptions<MyShopOnLineDataContext> options)
            : base(options)
        {
            DbContextOptionsBuilder dbContextOptionsBuilder = new DbContextOptionsBuilder(options);
            dbContextOptionsBuilder.EnableSensitiveDataLogging(true);
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.AddInterceptors(new MyShopOnLineDBContextSaveInterceptors());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(b =>
            {
                b.HasKey(p => p.Code);

                b.Property(p => p.Code)
                    .HasMaxLength(36);
                b.Property(p => p.Description)
                    .HasColumnType("NVARCHAR(MAX)");
                b.Property(p => p.Cost)
                    .HasColumnType("DECIMAL (8,2)");
                b.Property(p => p.Price)
                    .HasColumnType("DECIMAL (8,2)");
                b.Property(p => p.Weight)
                    .HasColumnType("DECIMAL (8,2)");

                b.HasData(
                    new Product()
                    {
                        Code = "ITEM00001",
                        Cost = 599.99m,
                        Description = "Laptop Computer",
                        Price = 779.99m,
                        Review = 5,
                        QuantityPerUnitPack = 1,
                        Weight = 1.7m
                    },
                    new Product()
                    {
                        Code = "ITEM00002",
                        Cost = 399.45m,
                        Description = "Desktop Computer",
                        Price = 519.29m,
                        Review = 5,
                        QuantityPerUnitPack = 1,
                        Weight = 12.5m
                    },
                    new Product()
                    {
                        Code = "ITEM00003",
                        Cost = 550,
                        Description = "HPC Graphic Card",
                        Price = 715,
                        Review = 5,
                        QuantityPerUnitPack = 1,
                        Weight = 2.5m
                    });
            });

            modelBuilder.Entity<Order>(b =>
            {
                b.HasKey(x => x.Number);
                b.Property(x => x.Total)
                    .HasColumnType("DECIMAL (8,2)");
                //  Model building for fields.
                //  This does not add any additional field mapping capabilities to EF Core, 
                //  it only allows the lambda methods to be used instead of always requiring the string methods.
                b.Property(x => x.Weight)
                    .HasColumnType("DECIMAL (8,2)");
            });

            modelBuilder.Entity<Customer>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Id)
                    .UseIdentityColumn()
                    .IsRequired();
                b.OwnsOne(x => x.BillingAddress, sa =>
                {
                    sa.Property(p => p.City).IsRequired();
                    sa.Property(p => p.Street).IsRequired();
                    sa.Property(p => p.ZipCode).IsRequired();
                });

                b.OwnsOne(x => x.ShippingAddress, sa =>
                {
                    sa.Property(p => p.City).IsRequired();
                    sa.Property(p => p.Street).IsRequired();
                    sa.Property(p => p.ZipCode).IsRequired();
                });

                b.Navigation(o => o.BillingAddress).IsRequired();
                b.Navigation(o => o.ShippingAddress);

                b.OwnsOne(x => x.BillingAddress).HasData(
                    new
                    {
                        CustomerId = 1,
                        City = "Zürich",
                        Street = "Bahnhofstrasse, 1",
                        ZipCode = "8000"
                    },
                    new
                    {
                        CustomerId = 2,
                        City = "Rome",
                        Street = "Piazza Porta Maggione 1",
                        ZipCode = "08100"
                    },
                    new
                    {
                        CustomerId = 3,
                        City = "New York City",
                        Street = "620 8th Ave #1",
                        ZipCode = "NY 10018"
                    }
                );

                b.HasData(
            new Customer()
            {
                Id = 1,
                Email = "pietro.libro@gmail.com",
                Fullname = "Pietro Libro"
            },
            new Customer()
            {
                Id = 2,
                Email = "pinco.pallo@outlook.com",
                Fullname = "Pinco Tizio Pallo"
            },
            new Customer()
            {
                Id = 3,
                Email = "john.Smith@yahoo.com",
                Fullname = "John Smith"
            }
        );
            });

            #region Many-to-Many relationship with Join Table.

            // Many-to-may relationship is automatically recognized, but a EF Core allows full
            // customization of the join table.

            modelBuilder.Entity<Order>()
                .HasMany(p => p.Products)
                .WithMany(p => p.Orders)
                .UsingEntity<OrderProduct>(
                 x =>
                    x.HasOne(op => op.Product)
                    .WithMany()
                    .HasForeignKey(op => op.ProductCode),
                 x =>
                    x.HasOne(op => op.Order)
                    .WithMany()
                    .HasForeignKey(op => op.OrderNumber),
                 x =>
                 {
                     x.Property(op => op.Quantity)
                        .HasDefaultValue(0);
                     x.HasKey(op => new { op.OrderNumber, op.ProductCode });
                 });

            #endregion 

            base.OnModelCreating(modelBuilder);
        }
    }

    public class MyShopOnLineDBContextSaveInterceptors : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            Console.WriteLine($"Saving changes for {eventData.Context.Database.GetConnectionString()}");

            return result;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine($"Saving changes asynchronously for {eventData.Context.Database.GetConnectionString()}");

            return new ValueTask<InterceptionResult<int>>(result);
        }
    }
}

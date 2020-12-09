using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyShopOnLine.Backend.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fullname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillingAddress_Street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BillingAddress_ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BillingAddress_City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BillingAddress_Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingAddress_Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingAddress_ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingAddress_City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingAddress_Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingAddressEqualsToBillingAddress = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    Cost = table.Column<decimal>(type: "DECIMAL (8,2)", nullable: false),
                    Price = table.Column<decimal>(type: "DECIMAL (8,2)", nullable: false),
                    Review = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<decimal>(type: "DECIMAL (8,2)", nullable: false),
                    QuantityPerUnitPack = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Number = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Total = table.Column<decimal>(type: "DECIMAL (8,2)", nullable: false),
                    Shipped = table.Column<bool>(type: "bit", nullable: false),
                    Delivered = table.Column<bool>(type: "bit", nullable: false),
                    ReadyForShipping = table.Column<bool>(type: "bit", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    ShippingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Weight = table.Column<decimal>(type: "DECIMAL (8,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Number);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderProducts",
                columns: table => new
                {
                    OrderNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderProducts", x => new { x.OrderNumber, x.ProductCode });
                    table.ForeignKey(
                        name: "FK_OrderProducts_Orders_OrderNumber",
                        column: x => x.OrderNumber,
                        principalTable: "Orders",
                        principalColumn: "Number",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderProducts_Products_ProductCode",
                        column: x => x.ProductCode,
                        principalTable: "Products",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Email", "Fullname", "ShippingAddressEqualsToBillingAddress", "BillingAddress_City", "BillingAddress_Note", "BillingAddress_Street", "BillingAddress_ZipCode" },
                values: new object[,]
                {
                    { 1, "pietro.libro@gmail.com", "Pietro Libro", true, "Zürich", null, "Bahnhofstrasse, 1", "8000" },
                    { 2, "pinco.pallo@outlook.com", "Pinco Tizio Pallo", true, "Rome", null, "Piazza Porta Maggione 1", "08100" },
                    { 3, "john.Smith@yahoo.com", "John Smith", true, "New York City", null, "620 8th Ave #1", "NY 10018" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Code", "Cost", "Description", "Price", "QuantityPerUnitPack", "Review", "Weight" },
                values: new object[,]
                {
                    { "ITEM00001", 599.99m, "Laptop Computer", 779.99m, 1, 5, 1.7m },
                    { "ITEM00002", 399.45m, "Desktop Computer", 519.29m, 1, 5, 12.5m },
                    { "ITEM00003", 550m, "HPC Graphic Card", 715m, 1, 5, 2.5m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderProducts_ProductCode",
                table: "OrderProducts",
                column: "ProductCode");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderProducts");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Customers] (
    [Id] int NOT NULL IDENTITY,
    [Email] nvarchar(max) NULL,
    [Fullname] nvarchar(max) NULL,
    [BillingAddress_Street] nvarchar(max) NOT NULL,
    [BillingAddress_ZipCode] nvarchar(max) NOT NULL,
    [BillingAddress_City] nvarchar(max) NOT NULL,
    [BillingAddress_Note] nvarchar(max) NULL,
    [ShippingAddress_Street] nvarchar(max) NULL,
    [ShippingAddress_ZipCode] nvarchar(max) NULL,
    [ShippingAddress_City] nvarchar(max) NULL,
    [ShippingAddress_Note] nvarchar(max) NULL,
    [ShippingAddressEqualsToBillingAddress] bit NOT NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Products] (
    [Code] nvarchar(36) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Cost] DECIMAL (8,2) NOT NULL,
    [Price] DECIMAL (8,2) NOT NULL,
    [Review] int NOT NULL,
    [Weight] DECIMAL (8,2) NOT NULL,
    [QuantityPerUnitPack] int NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Code])
);
GO

CREATE TABLE [Orders] (
    [Number] nvarchar(450) NOT NULL,
    [Total] DECIMAL (8,2) NOT NULL,
    [Shipped] bit NOT NULL,
    [Delivered] bit NOT NULL,
    [ReadyForShipping] bit NOT NULL,
    [OrderDate] datetime2 NOT NULL,
    [CustomerId] int NULL,
    [ShippingDate] datetime2 NULL,
    [DeliveryDate] datetime2 NULL,
    [Weight] DECIMAL (8,2) NOT NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Number]),
    CONSTRAINT [FK_Orders_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [OrderProducts] (
    [OrderNumber] nvarchar(450) NOT NULL,
    [ProductCode] nvarchar(36) NOT NULL,
    [Quantity] int NOT NULL DEFAULT 0,
    CONSTRAINT [PK_OrderProducts] PRIMARY KEY ([OrderNumber], [ProductCode]),
    CONSTRAINT [FK_OrderProducts_Orders_OrderNumber] FOREIGN KEY ([OrderNumber]) REFERENCES [Orders] ([Number]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderProducts_Products_ProductCode] FOREIGN KEY ([ProductCode]) REFERENCES [Products] ([Code]) ON DELETE CASCADE
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Email', N'Fullname', N'ShippingAddressEqualsToBillingAddress', N'BillingAddress_City', N'BillingAddress_Note', N'BillingAddress_Street', N'BillingAddress_ZipCode') AND [object_id] = OBJECT_ID(N'[Customers]'))
    SET IDENTITY_INSERT [Customers] ON;
INSERT INTO [Customers] ([Id], [Email], [Fullname], [ShippingAddressEqualsToBillingAddress], [BillingAddress_City], [BillingAddress_Note], [BillingAddress_Street], [BillingAddress_ZipCode])
VALUES (1, N'pietro.libro@gmail.com', N'Pietro Libro', CAST(1 AS bit), N'Zürich', NULL, N'Bahnhofstrasse, 1', N'8000'),
(2, N'pinco.pallo@outlook.com', N'Pinco Tizio Pallo', CAST(1 AS bit), N'Rome', NULL, N'Piazza Porta Maggione 1', N'08100'),
(3, N'john.Smith@yahoo.com', N'John Smith', CAST(1 AS bit), N'New York City', NULL, N'620 8th Ave #1', N'NY 10018');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Email', N'Fullname', N'ShippingAddressEqualsToBillingAddress', N'BillingAddress_City', N'BillingAddress_Note', N'BillingAddress_Street', N'BillingAddress_ZipCode') AND [object_id] = OBJECT_ID(N'[Customers]'))
    SET IDENTITY_INSERT [Customers] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Code', N'Cost', N'Description', N'Price', N'QuantityPerUnitPack', N'Review', N'Weight') AND [object_id] = OBJECT_ID(N'[Products]'))
    SET IDENTITY_INSERT [Products] ON;
INSERT INTO [Products] ([Code], [Cost], [Description], [Price], [QuantityPerUnitPack], [Review], [Weight])
VALUES (N'ITEM00001', 599.99, N'Laptop Computer', 779.99, 1, 5, 1.7),
(N'ITEM00002', 399.45, N'Desktop Computer', 519.29, 1, 5, 12.5),
(N'ITEM00003', 550.0, N'HPC Graphic Card', 715.0, 1, 5, 2.5);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Code', N'Cost', N'Description', N'Price', N'QuantityPerUnitPack', N'Review', N'Weight') AND [object_id] = OBJECT_ID(N'[Products]'))
    SET IDENTITY_INSERT [Products] OFF;
GO

CREATE INDEX [IX_OrderProducts_ProductCode] ON [OrderProducts] ([ProductCode]);
GO

CREATE INDEX [IX_Orders_CustomerId] ON [Orders] ([CustomerId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201207112601_InitialCreate', N'5.0.0');
GO

COMMIT;
GO


IF DB_ID(N'CafeteriaDB') IS NULL
    CREATE DATABASE CafeteriaDB;
GO

USE CafeteriaDB;
GO

SET XACT_ABORT ON;
BEGIN TRANSACTION;

CREATE TABLE dbo.Categories (
    Id int IDENTITY PRIMARY KEY,
    Name nvarchar(100) NOT NULL,
    Description nvarchar(max) NULL
);

CREATE TABLE dbo.Suppliers (
    Id int IDENTITY PRIMARY KEY,
    Name nvarchar(100) NOT NULL,
    Email nvarchar(254) NULL,
    Phone nvarchar(30) NULL
);

CREATE TABLE dbo.Products (
    Id int IDENTITY PRIMARY KEY,
    Name nvarchar(100) NOT NULL,
    Price decimal(18,2) NOT NULL CHECK (Price >= 0),
    Quantity int NOT NULL CHECK (Quantity >= 0),
    CategoryId int NOT NULL REFERENCES dbo.Categories(Id),
    SupplierId int NOT NULL REFERENCES dbo.Suppliers(Id)
);

CREATE TABLE dbo.PurchaseOrders (
    Id int IDENTITY PRIMARY KEY,
    OrderDate datetime2 NOT NULL,
    Status nvarchar(20) NOT NULL CHECK (Status IN (N'Pending', N'Received')),
    SupplierId int NOT NULL REFERENCES dbo.Suppliers(Id)
);

CREATE TABLE dbo.PurchaseOrderItems (
    Id int IDENTITY PRIMARY KEY,
    PurchaseOrderId int NOT NULL REFERENCES dbo.PurchaseOrders(Id),
    ProductId int NOT NULL REFERENCES dbo.Products(Id),
    Quantity int NOT NULL CHECK (Quantity > 0),
    UnitPrice decimal(18,2) NOT NULL CHECK (UnitPrice >= 0)
);

COMMIT TRANSACTION;
GO

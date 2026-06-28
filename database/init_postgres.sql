-- SimpleShopSystem PostgreSQL initialization script.
-- You can use EF Core migrations instead. This script mirrors the included EF Core model.

CREATE TABLE IF NOT EXISTS "Users" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(150) NOT NULL,
    "Email" VARCHAR(250) NOT NULL UNIQUE,
    "PasswordHash" TEXT NOT NULL,
    "Role" VARCHAR(30) NOT NULL DEFAULT 'User',
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS "Categories" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "Description" VARCHAR(250) NOT NULL,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "ModifiedAt" TIMESTAMP WITH TIME ZONE NULL,
    "OwnerId" INT NOT NULL REFERENCES "Users"("Id") ON DELETE RESTRICT
);

CREATE TABLE IF NOT EXISTS "Products" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(200) NOT NULL,
    "Description" TEXT NULL,
    "Price" NUMERIC(18,2) NOT NULL CHECK ("Price" >= 0),
    "StockQuantity" INT NOT NULL CHECK ("StockQuantity" >= 0),
    "ImageUrl" VARCHAR(500) NULL,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "ModifiedAt" TIMESTAMP WITH TIME ZONE NULL,
    "CategoryId" INT NOT NULL REFERENCES "Categories"("Id") ON DELETE RESTRICT,
    "OwnerId" INT NOT NULL REFERENCES "Users"("Id") ON DELETE RESTRICT
);

CREATE TABLE IF NOT EXISTS "Carts" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INT NOT NULL UNIQUE REFERENCES "Users"("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "CartItems" (
    "Id" SERIAL PRIMARY KEY,
    "CartId" INT NOT NULL REFERENCES "Carts"("Id") ON DELETE CASCADE,
    "ProductId" INT NOT NULL REFERENCES "Products"("Id") ON DELETE RESTRICT,
    "Quantity" INT NOT NULL CHECK ("Quantity" > 0),
    UNIQUE ("CartId", "ProductId")
);

CREATE TABLE IF NOT EXISTS "Orders" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INT NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "OrderDate" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "TotalAmount" NUMERIC(18,2) NOT NULL,
    "Status" VARCHAR(50) NOT NULL DEFAULT 'Placed'
);

CREATE TABLE IF NOT EXISTS "OrderItems" (
    "Id" SERIAL PRIMARY KEY,
    "OrderId" INT NOT NULL REFERENCES "Orders"("Id") ON DELETE CASCADE,
    "ProductId" INT NOT NULL REFERENCES "Products"("Id") ON DELETE RESTRICT,
    "ProductName" VARCHAR(200) NOT NULL,
    "ImageUrl" VARCHAR(500) NULL,
    "Quantity" INT NOT NULL CHECK ("Quantity" > 0),
    "UnitPrice" NUMERIC(18,2) NOT NULL CHECK ("UnitPrice" >= 0)
);

CREATE INDEX IF NOT EXISTS "IX_Categories_OwnerId" ON "Categories"("OwnerId");
CREATE INDEX IF NOT EXISTS "IX_Products_CategoryId" ON "Products"("CategoryId");
CREATE INDEX IF NOT EXISTS "IX_Products_OwnerId" ON "Products"("OwnerId");
CREATE INDEX IF NOT EXISTS "IX_Orders_UserId" ON "Orders"("UserId");

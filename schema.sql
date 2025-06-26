DROP DATABASE IF EXISTS bd_project;
GO

CREATE DATABASE bd_project;
GO

USE bd_project;
GO

-- Tabela Passwords 
CREATE TABLE Passwords (
    id INT IDENTITY(1,1) PRIMARY KEY,
    password_hash NVARCHAR(MAX) NOT NULL
);

-- Tabela Roles
CREATE TABLE Roles (
    id INT IDENTITY(1,1) PRIMARY KEY,
    role_name NVARCHAR(255) NOT NULL
);

-- Tabela Users
CREATE TABLE Users (
    id INT IDENTITY(1,1) PRIMARY KEY,
    username NVARCHAR(255) UNIQUE NOT NULL,
    password_id INT FOREIGN KEY REFERENCES Passwords(id) ON DELETE CASCADE,
    role_id INT FOREIGN KEY REFERENCES Roles(id) ON DELETE SET NULL
);

-- Tabela Products
CREATE TABLE Products (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(255) NOT NULL,
    description TEXT,
    price FLOAT NOT NULL,
    quantity INT NOT NULL CHECK (quantity >= 0)
);

-- Tabela OrderGroups
CREATE TABLE OrderGroups (
    id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL FOREIGN KEY REFERENCES Users(id) ON DELETE CASCADE,
    order_date DATETIME NOT NULL DEFAULT GETDATE(),
    [status] NVARCHAR(20) NOT NULL
);

-- Tabela OrderItems
CREATE TABLE OrderItems (
    id INT IDENTITY(1,1) PRIMARY KEY,
    order_group_id INT NOT NULL FOREIGN KEY REFERENCES OrderGroups(id) ON DELETE CASCADE,
    product_id INT NOT NULL FOREIGN KEY REFERENCES Products(id),
    quantity INT NOT NULL CHECK (quantity > 0)
);

-- Tabela Notifications
CREATE TABLE Notifications (
    id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL FOREIGN KEY REFERENCES Users(id) ON DELETE CASCADE,
    product_id INT NOT NULL FOREIGN KEY REFERENCES Products(id),
    should_send BIT NOT NULL,
    is_read BIT NOT NULL
);
GO

-- Trigger na restock magazynu
CREATE TRIGGER trg_NotifyRestock
ON Products
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE n
    SET n.should_send = 1,
        n.is_read = 0
    FROM Notifications n
    JOIN inserted i ON n.product_id = i.id
    JOIN deleted d ON d.id = i.id
    WHERE d.quantity = 0 AND i.quantity > 0;
END;
GO

-- Trigger na spóźnionych klientów, którzy nie zdążyli przeczytać powiadomienia, zanim ktoś kupił dany produkt
CREATE TRIGGER trg_NotifyOutOfStock
ON Products
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE n
    SET n.should_send = 0
    FROM Notifications n
    JOIN inserted i ON n.product_id = i.id
    JOIN deleted d ON d.id = i.id
    WHERE i.quantity = 0 AND n.is_read = 0;
END;
GO

-- Trigger na aktualizację daty modyfikacji koszyka
CREATE TRIGGER trg_UpdateCartOrderDate
ON OrderItems
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE og
    SET order_date = GETDATE()
    FROM OrderGroups og
    JOIN inserted i ON og.id = i.order_group_id
    WHERE og.status = 'cart';
END;
GO

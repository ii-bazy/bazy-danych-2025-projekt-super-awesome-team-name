-- DROP DATABASE IF EXISTS bd_project;
-- GO

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
    username NVARCHAR(255) NOT NULL,
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
    order_date DATETIME NOT NULL DEFAULT GETDATE()
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
    is_read BIT NULL
);


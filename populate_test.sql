USE bd_project;

INSERT INTO Roles (role_name) VALUES ('Admin'), ('Customer'); -- rethink roles?

INSERT INTO Passwords (password_hash) VALUES
('hash1'), ('hash2'), ('hash3'), ('hash4'), ('hash5'), ('hania'); -- will change that later 

INSERT INTO Users (username, password_id, role_id) VALUES
('alice', 1, 2),
('bob', 2, 2),
('carol', 3, 2),
('dave', 4, 2),
('eve', 5, 2),
('hania', 6, 1);

INSERT INTO Products (name, description, price, quantity) VALUES
('Laptop', 'Gaming laptop with RTX 4060', 5499.99, 10),
('Graphics Card', 'High-end GPU, temporarily unavailable', 2999.00, 0),
('Smartphone', 'Android phone 128GB', 1999.00, 25),
('Keyboard', 'Mechanical keyboard RGB', 399.99, 15),
('Mouse', 'Wireless mouse', 149.90, 30),
('Monitor', '27-inch IPS display', 1199.49, 12),
('Headphones', 'Noise-cancelling headphones', 499.00, 8),
('Webcam', '1080p webcam', 249.99, 20),
('Desk lamp', 'LED lamp with USB', 89.90, 50),
('Chair', 'Ergonomic office chair', 899.00, 5),
('USB-C Hub', 'Multi-port USB hub', 149.00, 40),
('External SSD', '1TB USB 3.1 SSD', 599.00, 7),
('Microphone', 'USB condenser mic', 349.00, 9);

INSERT INTO OrderGroups (user_id, order_date) VALUES
(1, '2024-01-15 10:00:00'),
(2, '2024-02-10 11:45:00'),
(2, '2024-03-05 09:20:00'),
(3, '2024-04-12 15:30:00'),
(4, '2024-05-01 08:10:00');

INSERT INTO OrderItems (order_group_id, product_id, quantity) VALUES
(1, 1, 1),
(1, 3, 2),
(2, 2, 1),
(3, 4, 3),
(4, 5, 1),
(4, 6, 1),
(5, 1, 1);

INSERT INTO Notifications (user_id, product_id, should_send, is_read) VALUES
(1, 2, 0, 0),
(2, 3, 1, 0),
(3, 4, 1, 1);

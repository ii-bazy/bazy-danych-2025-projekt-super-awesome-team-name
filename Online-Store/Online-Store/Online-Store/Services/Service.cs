using Microsoft.EntityFrameworkCore.Storage;
using Online_Store.Data;
using Online_Store.Data.Models;
using Online_Store.Models;
using System.Runtime.InteropServices;

namespace Online_Store.Services
{
    public class BuyResult : IBuyResult
    {
        public BuyResult() 
        {
            Succes = true;
        }

        public BuyResult(string itemName)
        {
            Succes = false;
            ItemName = itemName;
        }
        public bool Succes { get; }
        public string? ItemName { get; }
    }
    public class Service : IService
    {
        private readonly IUnitOfWork _unitOfWork;

        private ViewProduct ProductToViewProduct(Product product)
        {
            return new ViewProduct()
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity
            };
        }
        private ViewUser UserToViewUser(User user)
        {
            return new ViewUser()
            {
                Username = user.Username
            };
        }
        private ViewOrder OrderItemToViewOrder(OrderItem orderItem)
        {
            return new ViewOrder()
            {
                ProductName = orderItem.Product.Name,
                Quantity = orderItem.Quantity,
                ProductPrice = orderItem.Product.Price
            };
        }
        private ViewOrderGroup OrderGroupToViewOrderGroup(OrderGroup orderGroup)
        {
            var viewOrderGroup = new ViewOrderGroup()
            {
                Username = orderGroup.User.Username,
                DateOfPurchase = DateOnly.FromDateTime(orderGroup.OrderDate),
                Orders = new List<ViewOrder>()
            };  

            foreach (OrderItem orderItem in orderGroup.OrderItems)
                viewOrderGroup.Orders.Add(this.OrderItemToViewOrder(orderItem));

            return viewOrderGroup;
        }
        private ViewCartItem OrderItemToViewCartItem(OrderItem orderItem)
        {
            return new ViewCartItem()
            {
                Name = orderItem.Product.Name,
                Price = orderItem.Product.Price,
                Quantity = orderItem.Quantity
            };
        }
        private ViewNotification NotificationToViewNotification(Notification notification)
        {
            return new ViewNotification() { ItemName = notification.Product.Name, IsRead = notification.IsRead };
        }

        private Notification CreateNotification(Product product, User user)
        {
            return new Notification()
            {
                ShouldSend = false,
                IsRead = false,
                User = user,
                Product = product
            };
        }


        public Service(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<ViewProduct> GetProducts()
        {
            List<ViewProduct> viewProducts = new List<ViewProduct>();

            foreach (Product product in _unitOfWork.Products.GetAll())
                viewProducts.Add(this.ProductToViewProduct(product));
            
            return viewProducts;
        }

        public User? GetByUsernameAndPassword(string username, string hashedPassword)
        {
            var user = _unitOfWork.Users.GetByUsername(username);
            if (user == null) return null;

            if (user.Password.PasswordHash == hashedPassword) return user;
            return null;
        }
        public User? GetByUsername(string username)
        {
            return _unitOfWork.Users.GetByUsername(username);
        }
        public bool IsUsernameUsed(string username)
        {
            User user = _unitOfWork.Users.GetByUsername(username);
            if (user != null) return true;
            return false;
        }
        public IBuyResult BuyCart(string username)
        {
            var cart = _unitOfWork.OrderGroups.GetByUsernameAndStatus(username, "cart");
            var user = _unitOfWork.Users.GetByUsername(username);

            IBuyResult res = null;
            
            foreach (var item in cart.OrderItems)
                if (item.Quantity > item.Product.Quantity)
                {
                    res = new BuyResult(item.Product.Name);
                    var notification = this.CreateNotification(item.Product, user);
                    _unitOfWork.Notifications.Add(notification);
                }

            if (res != null)
            {
                _unitOfWork.Complete();
                return res;
            }

            foreach (var item in cart.OrderItems)
            {
                var product = item.Product;
                product.Quantity -= item.Quantity;

                _unitOfWork.Products.Update(product);
            }

            cart.Status = "payed";
            cart.OrderDate = DateTime.Now;
            _unitOfWork.OrderGroups.Update(cart);

            var newCart = new OrderGroup()
            {
                UserId = cart.UserId,
                Status = "cart"
            };
            _unitOfWork.OrderGroups.Add(newCart);

            _unitOfWork.Complete();

            return new BuyResult();
        }
        public void AddToCart(string username, int productId)
        {
            var cart = _unitOfWork.OrderGroups.GetByUsernameAndStatus(username, "cart");
            var item = cart.OrderItems.FirstOrDefault(oi => oi.ProductId == productId);

            if (item != null)
            {
                item.Quantity += 1;
                _unitOfWork.OrderItems.Update(item);
            }
            else
            {
                var product = _unitOfWork.Products.GetById(productId);
                var orderItem = new OrderItem()
                {
                    Product = product,
                    Quantity = 1,

                };
                cart.OrderItems.Add(orderItem);

                _unitOfWork.OrderItems.Add(orderItem);
            }
            _unitOfWork.Complete();

        }

        public void ChangeNotificationIsRead(int id)
        {
            var notification = _unitOfWork.Notifications.GetById(id);
            notification.IsRead ^= true;
            _unitOfWork.Notifications.Update(notification);

            _unitOfWork.Complete();
        }
        public void AddUser(string username, string hashedPassword, string roleName)
        {
            var password = new Password
            {
                PasswordHash = hashedPassword
            };

            var role = _unitOfWork.Roles.GetByRoleName(roleName);

            var user = new User
            {
                Username = username,
                Password = password,
                Role = role
            };

            var cart = new OrderGroup
            {
                User = user,
                Status = "cart"
            };

            _unitOfWork.Users.Add(user);
            _unitOfWork.OrderGroups.Add(cart);

            _unitOfWork.Complete();
        }

        public void DeleteUser(int id)
        {
            var user = _unitOfWork.Users.GetById(id);
            _unitOfWork.Users.Remove(user);
            
            var password = _unitOfWork.Passwords.GetById((int) user.PasswordId);
            _unitOfWork.Passwords.Remove(password);

            _unitOfWork.Complete();
        }
        public void AddProduct(string name, string description, double price, int quantity)
        {
            var product = new Product
            {
                Name = name,
                Description = description,
                Price = price,
                Quantity = quantity

            };

            _unitOfWork.Products.Add(product);

            _unitOfWork.Complete();
        }

        public void UpdateProduct(int id, string name, string description, double price, int quantity)
        {
            var product = _unitOfWork.Products.GetById(id);
            product.Description = description;
            product.Price = price;
            product.Quantity = quantity;
            product.Name = name;

            _unitOfWork.Products.Update(product);

            _unitOfWork.Complete();
        }

        public void DeleteProduct(int id)
        {
            var product = _unitOfWork.Products.GetById(id);
            _unitOfWork.Products.Remove(product);

            _unitOfWork.Complete();
        }

        public void DeleteOrderGroup(int id)
        {
            var orderGroup = _unitOfWork.OrderGroups.GetById(id);
            _unitOfWork.OrderGroups.Remove(orderGroup);

            _unitOfWork.Complete();
        }

        public void DeleteOrderItem(int id)
        {
            var orderItem = _unitOfWork.OrderItems.GetById(id);
            _unitOfWork.OrderItems.Remove(orderItem);

            _unitOfWork.Complete();
        }

        public Dictionary<int, ViewProduct> GetIdViewProducts()
        {
            var viewProducts = new Dictionary<int, ViewProduct>();

            foreach (var product in _unitOfWork.Products.GetAll())
                viewProducts[product.Id] = this.ProductToViewProduct(product);

            return viewProducts;
        }
        public Dictionary<int, ViewUser> GetIdViewUsers()
        {
            var viewUser = new Dictionary<int, ViewUser>();

            foreach (var user in _unitOfWork.Users.GetAll())
                viewUser[user.Id] = this.UserToViewUser(user);

            return viewUser;
        }
        public Dictionary<int, ViewOrderGroup> GetIdPayedOrderGroups()
        {
            var viewOrderGroup = new Dictionary<int, ViewOrderGroup>();

            foreach (var orderGroup in _unitOfWork.OrderGroups.GetAllPayed())
                viewOrderGroup[orderGroup.Id] = this.OrderGroupToViewOrderGroup(orderGroup);

            return viewOrderGroup;
        }
        public Dictionary<int, ViewCartItem> GetCartItems(string username)
        {
            Dictionary<int, ViewCartItem> viewCartItems = new Dictionary<int, ViewCartItem>();

            var cart = _unitOfWork.OrderGroups.GetByUsernameAndStatus(username, "cart");
            if (cart != null)
                foreach (var item in cart.OrderItems)
                    viewCartItems.Add(item.Id, this.OrderItemToViewCartItem(item));

            return viewCartItems;
        }
        public Dictionary<int, ViewNotification> GetIdSendNotifications(string username)
        {
            var viewNotifications = new Dictionary<int, ViewNotification>();

            foreach (var notification in _unitOfWork.Notifications.GetUsersRead(username))
                viewNotifications.Add(notification.Id, NotificationToViewNotification(notification));

            return viewNotifications;
        }

        public IDbContextTransaction BeginTransaction()
        {
            return _unitOfWork.BeginTransaction();
        }
    }
}

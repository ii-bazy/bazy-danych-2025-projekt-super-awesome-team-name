using Microsoft.EntityFrameworkCore.Storage;
using Online_Store.Data;
using Online_Store.Data.Models;
using Online_Store.Models;

namespace Online_Store.Services
{
    public class Service : IService
    {
        private readonly IUnitOfWork _unitOfWork;

        private ViewProduct ProductToViewProduct(Product product)
        {
            return new ViewProduct()
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price
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

        public bool IsUsernameUsed(string username)
        {
            User user = _unitOfWork.Users.GetByUsername(username);
            if (user != null) return true;
            return false;
        }
        public string BuyCart(string username)
        {
            var cart = _unitOfWork.OrderGroups.GetByUsernameAndStatus(username, "cart");

            foreach (var item in cart.OrderItems)
                if (item.Quantity > item.Product.Quantity)
                    return $"At least one of items in your cart ({item.Product.Name}) isn't avaible now. You will receive a notification as it appears. For now in order to buy cart, remove that product.";

            foreach (var item in cart.OrderItems)
            {
                var product = item.Product;
                product.Quantity -= item.Quantity;

                _unitOfWork.Products.Update(product);
            }

            cart.Status = "payed";
            _unitOfWork.OrderGroups.Update(cart);

            var newCart = new OrderGroup()
            {
                UserId = cart.UserId,
                Status = "cart"
            };
            _unitOfWork.OrderGroups.Add(newCart);

            _unitOfWork.Complete();

            return "Your products have been succefuly bought.";
        }

        // TODO: More than one item case
        public void AddToCart(string username, int productId)
        {
            var cart = _unitOfWork.OrderGroups.GetByUsernameAndStatus(username, "cart");
            var product = _unitOfWork.Products.GetById(productId);
            var orderItem = new OrderItem()
            {
                Product = product,
                Quantity = 1,

            };
            cart.OrderItems.Add(orderItem);

            _unitOfWork.OrderItems.Add(orderItem);

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
        public void AddProduct(string name, string description, float price, int quantity)
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

        public void UpdateProduct(int id, string name, string description, float price, int quantity)
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

        public Dictionary<int, ViewOrderGroup> GetIdOrderGroups()
        {
            var viewOrderGroup = new Dictionary<int, ViewOrderGroup>();

            foreach (var orderGroup in _unitOfWork.OrderGroups.GetAll())
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

        public IDbContextTransaction BeginTransaction()
        {
            return _unitOfWork.BeginTransaction();
        }
    }
}

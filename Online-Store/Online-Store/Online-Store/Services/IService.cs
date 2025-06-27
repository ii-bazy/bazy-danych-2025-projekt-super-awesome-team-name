using Microsoft.EntityFrameworkCore.Storage;
using Online_Store.Data.Models;
using Online_Store.Models;

namespace Online_Store.Services
{
    public interface IBuyResult
    {
        public bool Succes { get; }
        public string? ItemName { get; }
    }
    public interface IService
    {
        public IEnumerable<ViewProduct> GetProducts();
        
        public bool IsUsernameUsed(string username);
        public IBuyResult BuyCart(string username);
        public void AddToCart(string username, int productId);
        public void ChangeNotificationIsRead(int id);
        public User? GetByUsernameAndPassword(string username, string hashedPassword);
        public User? GetByUsername(string username);

        public void AddUser(string username, string hashedPassword, string roleName);
        public void AddProduct(string name, string description, double price, int quantity);

        public void UpdateProduct(int id, string name, string description, double price, int quantity);

        public void DeleteUser(int id);
        public void DeleteProduct(int id);
        public void DeleteOrderGroup(int id);
        public void DeleteOrderItem(int id);

        public Dictionary<int, ViewProduct> GetIdViewProducts();
        public Dictionary<int, ViewUser> GetIdViewUsers();
        public Dictionary<int, ViewOrderGroup> GetIdPayedOrderGroups();
        public Dictionary<int, ViewCartItem> GetCartItems(string username);

        public Dictionary<int, ViewNotification> GetIdSendNotifications(string username);

        public IDbContextTransaction BeginTransaction();
    }
}

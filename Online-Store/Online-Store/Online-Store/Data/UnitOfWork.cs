using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Online_Store.Data.Repositories;

namespace Online_Store.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private AppDbContext _context;
        public INotificationRepository Notifications { get; }
        public IOrderGroupRepository OrderGroups { get; }
        public IOrderItemRepository OrderItems { get; }
        public IPasswordRepository Passwords { get; }
        public IProductRepository Products { get; }
        public IRoleRepository Roles { get; }
        public IUserRepository Users { get; }

        public UnitOfWork(AppDbContext context)
        {
            this._context = context;
            Notifications = new NotificationRepository(_context);
            OrderGroups = new OrderGroupRepository(_context);
            OrderItems = new OrderItemRepository(_context);
            Passwords = new PasswordRepository(_context);
            Products = new ProductRepository(_context);
            Roles = new RoleRepository(_context);
            Users = new UserRepository(_context);
        }

        public IDbContextTransaction BeginTransaction()
        {
            return _context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
        }

        public int Complete() => _context.SaveChanges();

        public void Dispose() => _context.Dispose();
    }
}

using Microsoft.EntityFrameworkCore.Storage;
using Online_Store.Data.Repositories;

namespace Online_Store.Data
{
    public interface IUnitOfWork : IDisposable
    {
        INotificationRepository Notifications { get; }
        IOrderGroupRepository OrderGroups { get; }
        IOrderItemRepository OrderItems { get; }
        IPasswordRepository Passwords { get; }
        IProductRepository Products { get; }
        IRoleRepository Roles { get; }
        IUserRepository Users { get; }

        public IDbContextTransaction BeginTransaction();
        int Complete();
    }
}

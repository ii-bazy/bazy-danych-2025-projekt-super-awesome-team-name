using Microsoft.EntityFrameworkCore;
using Online_Store.Data.Models;

namespace Online_Store.Data.Repositories
{
    public interface INotificationRepository
    {
        Notification GetById(int id);
        IEnumerable<Notification> GetAll();
        IEnumerable<Notification> GetUsersRead(string username);
        void Add(Notification Notification);
        void Update(Notification Notification);
        void Remove(Notification Notification);
    }

    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public Notification GetById(int id) => _context.Notifications.Find(id);

        public IEnumerable<Notification> GetUsersRead(string username) =>
            _context.Notifications
            .Where(n => n.ShouldSend)
            .Include(n => n.User)
            .Where(n => n.User.Username == username)
            .Include(n => n.Product)
            .ToList();

        public IEnumerable<Notification> GetAll() => _context.Notifications.ToList();

        public void Add(Notification Notification) => _context.Notifications.Add(Notification);

        public void Update(Notification notification) => _context.Notifications.Update(notification);

        public void Remove(Notification Notification) => _context.Notifications.Remove(Notification);
    }
}

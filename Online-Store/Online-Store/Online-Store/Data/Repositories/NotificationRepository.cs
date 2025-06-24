using Online_Store.Data.Models;

namespace Online_Store.Data.Repositories
{
    public interface INotificationRepository
    {
        Notification GetById(int id);
        IEnumerable<Notification> GetAll();
        void Add(Notification Notification);
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

        public IEnumerable<Notification> GetAll() => _context.Notifications.ToList();

        public void Add(Notification Notification) => _context.Notifications.Add(Notification);

        public void Remove(Notification Notification) => _context.Notifications.Remove(Notification);
    }
}

using Online_Store.Data.Models;
using Online_Store.Models;

namespace Online_Store.Data.Repositories
{
    public interface IOrderItemRepository
    {
        OrderItem GetById(int id);
        IEnumerable<OrderItem> GetAll();
        void Add(OrderItem OrderItem);
        void Remove(OrderItem OrderItem);
    }

    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly AppDbContext _context;

        public OrderItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public OrderItem GetById(int id) => _context.OrderItems.Find(id);

        public IEnumerable<OrderItem> GetAll() => _context.OrderItems.ToList();

        public void Add(OrderItem OrderItem) => _context.OrderItems.Add(OrderItem);

        public void Remove(OrderItem OrderItem) => _context.OrderItems.Remove(OrderItem);
    }
}

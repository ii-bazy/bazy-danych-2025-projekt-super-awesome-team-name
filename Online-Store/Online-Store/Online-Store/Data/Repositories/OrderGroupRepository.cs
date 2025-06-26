using Microsoft.EntityFrameworkCore;
using Online_Store.Data.Models;
using Online_Store.Models;

namespace Online_Store.Data.Repositories
{
    public interface IOrderGroupRepository
    {
        OrderGroup GetById(int id);

        OrderGroup GetByUsernameAndStatus(string username, string status);
        IEnumerable<OrderGroup> GetAll();
        IEnumerable<OrderGroup> GetAllPayed();
        void Add(OrderGroup OrderGroup);
        void Remove(OrderGroup OrderGroup);
        void Update(OrderGroup OrderGroup);
    }

    public class OrderGroupRepository : IOrderGroupRepository
    {
        private readonly AppDbContext _context;

        public OrderGroupRepository(AppDbContext context)
        {
            _context = context;
        }

        public OrderGroup GetById(int id) => _context.OrderGroups.Find(id);

        public OrderGroup GetByUsernameAndStatus(string username, string status) => _context.OrderGroups
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Where(o => o.Status == status)
            .FirstOrDefault(o => o.User.Username == username);
            
        public IEnumerable<OrderGroup> GetAll() => _context.OrderGroups.ToList();

        public IEnumerable<OrderGroup> GetAllPayed() => 
            _context.OrderGroups
            .Where(o => o.Status == "payed")
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ToList();

        public void Add(OrderGroup OrderGroup) => _context.OrderGroups.Add(OrderGroup);

        public void Remove(OrderGroup OrderGroup) => _context.OrderGroups.Remove(OrderGroup);

        public void Update(OrderGroup OrderGroup) => _context.OrderGroups.Update(OrderGroup);
    }
}

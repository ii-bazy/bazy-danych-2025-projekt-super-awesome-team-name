using Online_Store.Data.Models;
using Microsoft.EntityFrameworkCore;


namespace Online_Store.Data.Repositories
{
    public interface IUserRepository
    {
        User GetById(int id);

        User? GetByUsername(string username);
        IEnumerable<User> GetAll();
        void Add(User User);
        void Remove(User User);
    }

    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public User GetById(int id) => _context.Users.Find(id);

        public User? GetByUsername(string username) => _context.Users
            .Include(u => u.Password)
            .FirstOrDefault(u => u.Username == username);

        public IEnumerable<User> GetAll() => _context.Users.ToList();

        public void Add(User User) => _context.Users.Add(User);

        public void Remove(User User) => _context.Users.Remove(User);

        
    }
}

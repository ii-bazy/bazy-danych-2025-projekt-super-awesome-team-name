using Online_Store.Data.Models;

namespace Online_Store.Data.Repositories
{
    public interface IPasswordRepository
    {
        Password GetById(int id);
        IEnumerable<Password> GetAll();
        void Add(Password Password);
        void Remove(Password Password);
    }

    public class PasswordRepository : IPasswordRepository
    {
        private readonly AppDbContext _context;

        public PasswordRepository(AppDbContext context)
        {
            _context = context;
        }

        public Password GetById(int id) => _context.Passwords.Find(id);

        public IEnumerable<Password> GetAll() => _context.Passwords.ToList();

        public void Add(Password Password) => _context.Passwords.Add(Password);

        public void Remove(Password Password) => _context.Passwords.Remove(Password);
    }
}

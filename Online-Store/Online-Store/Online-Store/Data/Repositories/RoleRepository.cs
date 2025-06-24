using Online_Store.Data.Models;

namespace Online_Store.Data.Repositories
{
    public interface IRoleRepository
    {
        Role GetById(int id);
        public Role GetByRoleName(string roleName);
        IEnumerable<Role> GetAll();
        void Add(Role Role);
        void Remove(Role Role);
    }

    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public Role GetById(int id) => _context.Roles.Find(id);

        public Role GetByRoleName(string roleName) => _context.Roles.FirstOrDefault(r => r.RoleName == roleName);

        public IEnumerable<Role> GetAll() => _context.Roles.ToList();

        public void Add(Role Role) => _context.Roles.Add(Role);

        public void Remove(Role Role) => _context.Roles.Remove(Role);
    }
}

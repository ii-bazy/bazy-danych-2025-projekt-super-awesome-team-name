using Online_Store.Data.Models;

namespace Online_Store.Data.Repositories
{
    public interface IProductRepository
    {
        Product GetById(int id);
        IEnumerable<Product> GetAll();
        void Add(Product product);
        void Remove(Product product);
        void Update(Product product);
    }

    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public Product GetById(int id) => _context.Products.Find(id);

        public IEnumerable<Product> GetAll() => _context.Products.ToList();

        public void Add(Product product) => _context.Products.Add(product);

        public void Remove(Product product) => _context.Products.Remove(product);

        public void Update(Product product) => _context.Products.Update(product);
    }
}

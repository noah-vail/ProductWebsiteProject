using GraphicsCardProject.DAL.DomainClasses;
using Microsoft.EntityFrameworkCore;

namespace GraphicsCardProject.DAL.DAO
{
    public class CategoryDAO
    {
        private readonly AppDbContext _db;
        public CategoryDAO(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Category>> GetAll()
        {
            return await _db.Categories!.ToListAsync();
        }

        public async Task<List<Product>> GetAllByCategory(int id)
        {
            return await _db.Products!.Where(item => item.Category!.Id == id).ToListAsync();
        }
    }
}

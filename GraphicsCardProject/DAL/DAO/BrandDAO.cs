using GraphicsCardProject.DAL.DomainClasses;
using Microsoft.EntityFrameworkCore;

namespace GraphicsCardProject.DAL.DAO
{
    public class BrandDAO
    {
        private readonly AppDbContext _db;
        public BrandDAO(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Brand>> GetAll()
        {
            return await _db.Brands!.ToListAsync();
        }
    }
}

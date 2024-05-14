using GraphicsCardProject.DAL.DomainClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace GraphicsCardProject.DAL.DAO
{
    public class ProductDAO
    {
        private readonly AppDbContext _db;
        public ProductDAO(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Product>> GetAll()
        {
            return await _db.Products!.ToListAsync();
        }

        public async Task<List<Product>> GetAllByBrand(int id)
        {
            return await _db.Products!.Where(item => item.Brand!.Id == id).ToListAsync(); ;
        }

        
    }
}

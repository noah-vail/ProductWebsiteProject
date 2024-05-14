using GraphicsCardProject.DAL.DomainClasses;
using Microsoft.EntityFrameworkCore;

namespace GraphicsCardProject.DAL.DAO
{
    public class CustomerDAO
    {
        private readonly AppDbContext _db;
        public CustomerDAO(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Customer> Register(Customer customer)
        {
            await _db.Customers!.AddAsync(customer);
            await _db.SaveChangesAsync();
            return customer;
        }

        public async Task<Customer?> GetByEmail(string? email)
        {
            Customer? cust = await _db.Customers!.FirstOrDefaultAsync(c => c.Email == email);
            return cust;
        }
    }
}

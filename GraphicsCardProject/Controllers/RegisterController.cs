using GraphicsCardProject.DAL;
using GraphicsCardProject.DAL.DAO;
using GraphicsCardProject.DAL.DomainClasses;
using GraphicsCardProject.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace GraphicsCardProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        readonly AppDbContext _db;
        public RegisterController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<CustomerHelper>> Index(CustomerHelper helper)
        {
            CustomerDAO dao = new(_db);
            Customer? already = await dao.GetByEmail(helper.Email);
            if (already == null)
            {
                int saltBytes = 64;
                int hashSize = 64;
                int iterations = 10000;

                string salted = HashSalt.GenerateSalt(saltBytes);

                string hashed = HashSalt.HashPassword(helper.Password!, salted, iterations, hashSize);
                helper.Password = "";

                Customer dbCustomer = new();
                dbCustomer.FirstName = helper.FirstName!;
                dbCustomer.LastName = helper.LastName!;
                dbCustomer.Email = helper.Email!;
                dbCustomer.Hash = hashed;
                dbCustomer.Salt = salted;

                dbCustomer = await dao.Register(dbCustomer);

                if (dbCustomer.Id > 0)
                {
                    helper.Token = "customer registered";
                }
                else
                {
                    helper.Token = "customer registration failed";
                }
            }
            else
            {
                helper.Token = "user registration failed - email already in use";
            }
            return helper;
        }
    }

    public class HashSalt
    {
        public string? Hash { get; set; }
        public string? Salt { get; set; }

        public static string GenerateSalt(int size)
        {
            
            byte[] saltBytes = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(saltBytes);
            }

            return Convert.ToBase64String(saltBytes);
        }

        public static string HashPassword(string password, string salt, int iterations, int hashSize)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);

            using(var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, iterations, HashAlgorithmName.SHA256))
            {
                return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(hashSize));
            }
        }
    }
}

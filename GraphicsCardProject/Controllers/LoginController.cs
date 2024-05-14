using GraphicsCardProject.DAL;
using GraphicsCardProject.DAL.DAO;
using GraphicsCardProject.DAL.DomainClasses;
using GraphicsCardProject.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GraphicsCardProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        readonly AppDbContext _db;
        readonly IConfiguration _configuration;

        public LoginController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            this._configuration = config;
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<CustomerHelper>> Index(CustomerHelper helper)
        {
            CustomerDAO dao = new(_db);
            Customer? cust = await dao.GetByEmail(helper.Email);

            if (cust != null)
            {
                int hashSize = 64;
                if (VerifyPassword(helper.Password, cust.Hash!, cust.Salt!, hashSize))
                {
                    helper.Password = "";
                    var appSettings = _configuration.GetSection("AppSettings").GetValue<string>("Secret");

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(appSettings);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Name, cust.Id.ToString())
                        }),
                        Expires = DateTime.UtcNow.AddDays(14),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    string returnToken = tokenHandler.WriteToken(token);
                    helper.Token = returnToken;
                }
                else
                {
                    helper.Token = "username or password invalid - login failed";
                }
            }
            else
            {
                helper.Token = "no such user - login failed";
            }
            return helper;
        }

        public static bool VerifyPassword(string? enteredPassword, string storedHash, string storedSalt, int hashSize)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(enteredPassword!, saltBytes, 10000, HashAlgorithmName.SHA256);
            return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(hashSize)) == storedHash;
        }
    }
}

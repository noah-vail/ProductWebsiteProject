using GraphicsCardProject.DAL.DAO;
using GraphicsCardProject.DAL.DomainClasses;
using GraphicsCardProject.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GraphicsCardProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        readonly AppDbContext _db;
        public BrandController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<List<Brand>>> Index()
        {
            BrandDAO dao = new(_db);
            List<Brand> allBrands = await dao.GetAll();
            return allBrands;
        }
    }
}

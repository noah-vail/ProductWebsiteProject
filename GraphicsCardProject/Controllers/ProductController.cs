using GraphicsCardProject.DAL;
using GraphicsCardProject.DAL.DAO;
using GraphicsCardProject.DAL.DomainClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GraphicsCardProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        readonly AppDbContext _db;
        public ProductController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> Index()
        {
            ProductDAO dao = new(_db);
            List<Product> allProducts = await dao.GetAll();
            return allProducts;
        }

        [HttpGet]
        [Route("{brandid}")]
        public async Task<ActionResult<List<Product>>> GetAllBrands(int brandid)
        {
            ProductDAO dao = new(_db);
            List<Product> itemsForBrand = await dao.GetAllByBrand(brandid);
            return itemsForBrand;
        }
    }
}

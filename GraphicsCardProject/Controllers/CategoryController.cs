using GraphicsCardProject.DAL;
using GraphicsCardProject.DAL.DAO;
using GraphicsCardProject.DAL.DomainClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GraphicsCardProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        readonly AppDbContext _db;
        public CategoryController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<List<Category>>> Index()
        {
            CategoryDAO dao = new(_db);
            List<Category> allCategories =await dao.GetAll();
            return allCategories;
        }

        [HttpGet]
        [Route("{categoryid}")]
        public async Task<ActionResult<List<Product>>> GetAllProducts(int categoryid)
        {
            CategoryDAO dao = new(_db);
            List<Product> allProducts = await dao.GetAllByCategory(categoryid);
            return allProducts;
        }
    }
}

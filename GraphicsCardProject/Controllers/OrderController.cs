using GraphicsCardProject.DAL.DAO;
using GraphicsCardProject.DAL.DomainClasses;
using GraphicsCardProject.DAL;
using GraphicsCardProject.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GraphicsCardProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        readonly AppDbContext _db;
        public OrderController(AppDbContext db)
        {
            _db = db;
        }

        [Route("{email}")]
        [HttpGet]
        public async Task<ActionResult<List<Order>>> List(string email)
        {
            List<Order> orders;
            CustomerDAO cDao = new CustomerDAO(_db);
            Customer? orderOwner = await cDao.GetByEmail(email);
            OrderDAO oDao = new OrderDAO(_db);
            orders = await oDao.GetAll(orderOwner!.Id);
            return orders;
        }

        [Route("{orderid}/{email}")]
        [HttpGet]
        public async Task<ActionResult<List<OrderDetailsHelper>>> GetOrderDetails(int orderid, string email)
        {
            OrderDAO dao = new(_db);
            return await dao.GetOrderDetails(orderid, email);
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<string>> Index(OrderHelper helper)
        {
            string retVal;

            try
            {
                CustomerDAO cDao = new(_db);
                Customer? orderOwner = await cDao.GetByEmail(helper.Email);
                OrderDAO oDao = new(_db);
                int orderId = await oDao.AddOrder(orderOwner!.Id, helper.Selections!);

                retVal = orderId > 0
                    ? "Order " + orderId + " created! Goods Ordered!"
                    : "Order not in stock! Goods Backordered!";
            }
            catch (Exception ex)
            {
                retVal = "Order not saved " + ex.Message;
            }

            return retVal;
        }
    }
}

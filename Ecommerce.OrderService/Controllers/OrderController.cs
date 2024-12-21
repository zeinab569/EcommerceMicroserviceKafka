using Ecommerce.Model;
using Ecommerce.OrderService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(OrderDbContext dbContext) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<OrderModel>>> GetOrder()
        {
            return await dbContext.Orders.ToListAsync();
        }
    }
}

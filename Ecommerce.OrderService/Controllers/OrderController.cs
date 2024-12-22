using Confluent.Kafka;
using Ecommerce.Model;
using Ecommerce.OrderService.Data;
using Ecommerce.OrderService.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Ecommerce.OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(OrderDbContext dbContext,IKafkaProducer kafkaProducer) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<OrderModel>>> GetOrder()
        {
            return await dbContext.Orders.ToListAsync();
        }

        [HttpPost]
        public async Task<OrderModel> CreateOrder(OrderModel order)
        {
            order.OrderDate = DateTime.Now;
            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();

            //var orderMessage = new OrderMessage
            //{
            //    OrderId = order.Id,
            //    ProductId = order.ProductId,
            //    Quantity = order.Quantity
            //};

            await kafkaProducer.ProduceAsync("order-topic", new Message<string, string>
            {
                Key = order.Id.ToString(),
                Value = JsonSerializer.Serialize(order)
            });

            return order;
        }
    }
}

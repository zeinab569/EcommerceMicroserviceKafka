using Confluent.Kafka;
using Ecommerce.Common;
using Ecommerce.Model;
using Ecommerce.OrderService.Data;
using Newtonsoft.Json;

namespace Ecommerce.OrderService.Kafka;

public class OrderConsumer(IServiceProvider serviceProvider, IKafkaProducer kafkaProducer) : KafkaConsumer(topics)
{
    private static readonly string[] topics = ["payment-processed", "products-reservation-failed", "products-reservation-canceled"];
    private OrderDbContext GetDbContextcs()
    {
        var scope = serviceProvider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    }
    protected override async Task ConsumeAsync(ConsumeResult<string, string> consumeResult)
    {
        await base.ConsumeAsync(consumeResult);
        switch (consumeResult.Topic)
        {
            case "payment-processed":
                await HandelConfirmOrder(consumeResult.Message.Value);
                break;
            case "products-reservation-failed":
                await HandelConcelOrder(consumeResult.Message.Value);
                break;
            case "products-reservation-canceled":
                await HandelConcelOrder(consumeResult.Message.Value);
                break;
        }
    }
    public async Task HandelConfirmOrder(string value)
    {
        var orderMessage = JsonConvert.DeserializeObject<OrderMessage>(value);
        using var dbContext = GetDbContextcs();
        var order = await dbContext.Orders.FindAsync(orderMessage.OrderId);
        if (order != null)
        {
            order.Status = "Confirmed";
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task HandelConcelOrder(string value)
    {
        var orderMessage = JsonConvert.DeserializeObject<OrderMessage>(value);
        using var dbContext = GetDbContextcs();
        var order = await dbContext.Orders.FindAsync(orderMessage.OrderId);
        if (order != null)
        {
            order.Status = "Canceled";
            await dbContext.SaveChangesAsync();
        }
    }


}

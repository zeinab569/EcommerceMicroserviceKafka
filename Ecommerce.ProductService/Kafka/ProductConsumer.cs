using Confluent.Kafka;
using Ecommerce.Common;
using Ecommerce.Model;
using Ecommerce.ProductService.Data;
using Newtonsoft.Json;

namespace Ecommerce.ProductService.Kafka;

public class ProductConsumer(IServiceProvider serviceProvider, IKafkaProducer kafkaProducer) : KafkaConsumer(topics)
{
    private static readonly string[] topics = ["order-created", "payment-processed-failed"];

    private ProductDbContextcs GetDbContextcs()
    {
        var scope = serviceProvider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<ProductDbContextcs>();
    }
    protected override async Task ConsumeAsync(ConsumeResult<string, string> consumeResult)
    {
        await base.ConsumeAsync(consumeResult);
        switch (consumeResult.Topic)
        {
            case "order-created":
                await HandelOrderCreated(consumeResult.Message.Value);
                break;
            case "payment-processed-failed":
                await HandelPaymentFaild(consumeResult.Message.Value);
                break;
        }
    }

    public async Task HandelOrderCreated(string value)
    {
        var orderMessage = JsonConvert.DeserializeObject<OrderMessage>(value);
        var isReserved = await ReserveProducts(orderMessage);
        if (isReserved)
            await kafkaProducer.ProduceAsync("products-reserved", orderMessage);
        else
            await kafkaProducer.ProduceAsync("products-reservation-failed", orderMessage);

    }
    public async Task HandelPaymentFaild(string value)
    {
        var orderMessage = JsonConvert.DeserializeObject<OrderMessage>(value);
        using var dbContext = GetDbContextcs();
        var product = await dbContext.Products.FindAsync(orderMessage.ProductId);
        if (product != null)
        {
            product.Quantity += orderMessage.Quantity;
            await dbContext.SaveChangesAsync();
        }
        await kafkaProducer.ProduceAsync("products-reservation-canceled", orderMessage);

    }
    private async Task<bool> ReserveProducts(OrderMessage orderMessage)
    {
        using var dbContext = GetDbContextcs();
        var product = await dbContext.Products.FindAsync(orderMessage.ProductId);
        if (product != null && product.Quantity >= orderMessage.Quantity)
        {
            product.Quantity -= orderMessage.Quantity;
            await dbContext.SaveChangesAsync();
            return true;
        }
        return false;
    }
}

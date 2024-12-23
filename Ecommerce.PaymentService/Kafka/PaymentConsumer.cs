using Confluent.Kafka;
using Ecommerce.Common;
using Ecommerce.Model;
using Newtonsoft.Json;

namespace Ecommerce.PaymentService.Kafka;

public class PaymentConsumer(IKafkaProducer kafkaProducer) : KafkaConsumer(topics)
{
    private static readonly string[] topics = ["products-reserved"];
    protected override async Task ConsumeAsync(ConsumeResult<string, string> consumeResult)
    {
        await base.ConsumeAsync(consumeResult);
        switch (consumeResult.Topic)
        {
            case "products-reserved":
                await HandelProductsReserverd(consumeResult.Message.Value);
                break;
        }
    }

    public async Task HandelProductsReserverd(string value)
    {
        var orderMessage = JsonConvert.DeserializeObject<OrderMessage>(value);
        var isParmentProcessed = await PaymentProcessed(orderMessage);
        if (isParmentProcessed)
            await kafkaProducer.ProduceAsync("payment-processed", orderMessage);
        else
            await kafkaProducer.ProduceAsync("payment-processed-failed", orderMessage);

    }

    private async Task<bool> PaymentProcessed(OrderMessage orderMessage)
    {
        return true;
    }

}

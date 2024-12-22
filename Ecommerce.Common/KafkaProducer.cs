using Confluent.Kafka;
using Newtonsoft.Json;

namespace Ecommerce.Common;

public interface IKafkaProducer
{
    Task ProduceAsync(string topic, Object message);
}
public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<string, string> _producer;
    public KafkaProducer()
    {
        var config = new ConsumerConfig
        {
            GroupId = "order-group",
            BootstrapServers = "localhost:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        _producer = new ProducerBuilder<string, string>(config).Build();
    }
    public async Task ProduceAsync(string topic, object message)
    {
        var kafkaMessage = new Message<string, string> { Value = JsonConvert.SerializeObject(message) };
        await _producer.ProduceAsync(topic, kafkaMessage);
    }
}

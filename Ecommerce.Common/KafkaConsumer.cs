using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using static Confluent.Kafka.ConfigPropertyNames;

namespace Ecommerce.Common;

public class KafkaConsumer : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;
    public KafkaConsumer(string[] topics)
    {
        var config = new ConsumerConfig
        {
            GroupId = "order-group",
            BootstrapServers = "localhost:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest //oldest one in Queue
        };
        _consumer = new ConsumerBuilder<string, string>(config).Build();
        _consumer.Subscribe(topics);

    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            await HandelConsume(stoppingToken);
        }, stoppingToken);
    }

    private async Task HandelConsume(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var consumeResult = _consumer.Consume(stoppingToken);
            await ConsumeAsync(consumeResult );
        }
        _consumer.Dispose();
    }

    protected virtual  Task ConsumeAsync(ConsumeResult<string, string> consumeResult)
    {
        return Task.CompletedTask;
    }
}
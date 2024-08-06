using Confluent.Kafka;
using Microsoft.Extensions.Options;
using OtusHomework.Kafka;

namespace OtusHomework.CacheUpdateService
{
    public class Worker(IOptions<KafkaSettings> options) : BackgroundService
    {
        private readonly IOptions<KafkaSettings> options = options;

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            using var consumer = new ConsumerBuilder<string, string>(GetConsumerConfig()).Build();
            consumer.Subscribe("feed-posts");
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var consumerResult = consumer.Consume(ct);
                    if (consumerResult.IsPartitionEOF)
                    {
                        await Task.Delay(5000, ct);
                        continue;
                    }
                    ct.ThrowIfCancellationRequested();
                    consumer.StoreOffset(consumerResult);

                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            consumer.Close();
        }

        private ConsumerConfig GetConsumerConfig()
        {
            return new ConsumerConfig
            {
                GroupId = "CaseUpdateService",
                EnableAutoOffsetStore = false,
                EnableAutoCommit = true,
                EnablePartitionEof = true,
                AutoOffsetReset = AutoOffsetReset.Earliest,
#if DEBUG
                BootstrapServers = options.Value.Host_debug,
#else
                BootstrapServers = options.Value.Host,
#endif

            };
        }
    }
}

using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace OtusHomework.Kafka
{
    public class KafkaClientHandle
    {
        private readonly IProducer<byte[], byte[]> kafkaProducer;

        public KafkaClientHandle(IConfiguration config)
        {
            var conf = new ProducerConfig();
            //config.GetRequiredSection("Kafka:ProducerSettings").(conf);
            kafkaProducer = new ProducerBuilder<byte[], byte[]>(conf).Build();
        }

        public Handle Handle { get => kafkaProducer.Handle; }

        public void Dispose()
        {
            kafkaProducer.Flush();
            kafkaProducer.Dispose();
        }
    }
}

using System;
using System.Text;
using System.Text.Json;
using Shared.Models;
using RabbitMQ.Client;
using CurrencyProducer.API.Interface;
using System.Threading.Tasks;
using RabbitMQ.Client.Exceptions;

namespace CurrencyProducer.API.Services
{
    public class RabbitMqPublisher : IRabbitMqPublisher
    {
        private readonly ConnectionFactory _factory;

        public RabbitMqPublisher()
        {
            _factory = new ConnectionFactory()
            {
                //Uri = new Uri("amqps://username:password@host/vhost")
                Uri = new Uri("amqps://xcxyqxdi:xxxxxxxxxxxxxxxxxxxxxxxxxxxxxj@yak.lmq.cloudamqp.com/xcxyqxdi")

                
            };
        }

        public async Task PublishAsync(CurrencyRateMessage message)
        {
            try
            {

                using var connection = await _factory.CreateConnectionAsync(); // Await the async connection
                using var channel = await connection.CreateChannelAsync();  //connection.CreateModel();                 // Channel from the connection
          
                channel.QueueDeclareAsync("currency_queue", false, false, false);

                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                await channel.BasicPublishAsync(
                "",
                "currency_queue",
                body
                );

            }
            catch (BrokerUnreachableException ex)
            {
                Console.WriteLine($"[RabbitMQ ERROR] Broker unreachable: {ex.Message}");
                // Optionally: implement retry logic here
            }
            catch (OperationInterruptedException ex)
            {
                Console.WriteLine($"[RabbitMQ ERROR] Operation interrupted: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RabbitMQ ERROR] Unexpected error: {ex.Message}");
            }


        }
    }
}
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Shared.Models;

namespace CurrencyConsumer.API.Service
{
    public class CurrencyConsumer : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory()
            {
                Uri = new Uri("amqps://xcxyqxdi:xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx@yak.lmq.cloudamqp.com/xcxyqxdi")
            };

            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync("currency_queue", false, false, false);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                var message = JsonSerializer.Deserialize<CurrencyRateMessage>(json);

                Console.WriteLine($"Received: {message.Currency} = {message.Rate}");

                await Task.Yield();
            };

            await channel.BasicConsumeAsync("currency_queue", true, consumer);

            Console.WriteLine(" Consumer started...");

            // Keep service alive
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}

using Shared.Models;
using Shared.DTO;
using System;
using System.Text.Json;

namespace CurrencyProducer.API.Services
{
    public class CurrencyService
    {
        private readonly HttpClient _httpClient;
        private readonly RabbitMqPublisher _publisher;

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _publisher = new RabbitMqPublisher();
        }

        public async Task PublishAllRates()
        {
            var response = await _httpClient.GetStringAsync("https://open.er-api.com/v6/latest/USD");

            var json = JsonDocument.Parse(response);
            var rates = json.RootElement.GetProperty("rates");

            foreach (var item in rates.EnumerateObject())
            {
                var message = new CurrencyRateMessage
                {
                    Base = "USD",
                    Currency = item.Name,
                    Rate = item.Value.GetDecimal(),
                    Timestamp = DateTime.UtcNow
                };

                _publisher.PublishAsync(message);

                Console.WriteLine($"Published: {item.Name}");
            }
        }

        public async Task PublishRatesAsync()
        {
            var response = await GetAllRatesAsync();

            foreach (var rate in response.Rates)
            {
                var message = new CurrencyRateMessage
                {
                    Base = response.Base,
                    Currency = rate.Currency,
                    Rate = rate.Rate,
                    Timestamp = DateTime.UtcNow
                };

               await _publisher.PublishAsync(message);

                Console.WriteLine($"Published: {rate.Currency} = {rate.Rate}");
            }
        }
        public async Task<CurrencyRatesResponseDto> GetAllRatesAsync()
        {
            var response = await _httpClient.GetStringAsync("https://open.er-api.com/v6/latest/USD");

            var json = JsonDocument.Parse(response);

            var ratesElement = json.RootElement.GetProperty("rates");

            var ratesList = new List<CurrencyRateDto>();

            foreach (var item in ratesElement.EnumerateObject())
            {
                ratesList.Add(new CurrencyRateDto
                {
                    Currency = item.Name,
                    Rate = item.Value.GetDecimal()
                });
            }

            return new CurrencyRatesResponseDto
            {
                Base = "USD",
                Date = DateTime.UtcNow,
                Rates = ratesList
            };
        }
    }
}

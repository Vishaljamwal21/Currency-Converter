using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace CurrencyConverter.Services
{
    public class ExchangeRateService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ExchangeRateService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<Dictionary<string, decimal>> GetExchangeRatesAsync(string baseCurrency)
        {
            var baseUrl = _configuration["ExchangeRateAPI:BaseUrl"];
            var apiKey = _configuration["ExchangeRateAPI:ApiKey"];
            var url = $"{baseUrl}{baseCurrency}?access_key={apiKey}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                dynamic data = JsonConvert.DeserializeObject(json);

                if (data == null || data.rates == null)
                {
                    throw new Exception("Invalid response format.");
                }

                var rates = new Dictionary<string, decimal>();
                foreach (var rate in data.rates)
                {
                    rates.Add(rate.Name, (decimal)rate.Value);
                }

                return rates;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("An error occurred while fetching exchange rates.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing the exchange rates.", ex);
            }
        }

        public async Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            var rates = await GetExchangeRatesAsync(fromCurrency);
            if (!rates.ContainsKey(toCurrency))
            {
                throw new Exception($"Exchange rate for {toCurrency} not found.");
            }
            return rates[toCurrency];
        }

        public async Task<decimal> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency)
        {
            var rate = await GetExchangeRateAsync(fromCurrency, toCurrency);
            return amount * rate;
        }

    }
}

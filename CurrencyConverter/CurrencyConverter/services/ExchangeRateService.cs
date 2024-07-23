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

        public async Task<Dictionary<string, string>> GetCurrenciesAsync()
        {
            var url = "https://openexchangerates.org/api/currencies.json";
            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var currencies = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                return currencies;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("An error occurred while fetching currencies.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing currencies.", ex);
            }
        }

        public async Task<Dictionary<string, decimal>> GetHistoricalRatesAsync(DateTime date)
        {
            var baseUrl = _configuration["HistoricalExchangeRateAPI:BaseUrl"];
            var appId = _configuration["HistoricalExchangeRateAPI:AppId"];
            var url = $"{baseUrl}{date:yyyy-MM-dd}.json?app_id={appId}";
            using var httpClient = new HttpClient();
            try
            {
                var response = await httpClient.GetStringAsync(url);
                var result = JsonConvert.DeserializeObject<dynamic>(response);
                var rates = result?.rates;
                if (rates == null)
                {
                    return new Dictionary<string, decimal>();
                }
                return rates.ToObject<Dictionary<string, decimal>>();
            }
            catch (HttpRequestException ex)
            {
                return new Dictionary<string, decimal>();
            }
            catch (JsonException ex)
            {
                return new Dictionary<string, decimal>();
            }
        }

    }
}

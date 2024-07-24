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
        private readonly string _baseUrl;
        private readonly string _apiKey;
        private readonly string _currencyUrl;
        private readonly string _historicalBaseUrl;
        private readonly string _appId;

        public ExchangeRateService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ExchangeRateAPI:BaseUrl"];
            _apiKey = configuration["ExchangeRateAPI:ApiKey"];
            _currencyUrl = configuration["HistoricalExchangeRateAPI:CurrencyUrl"];
            _historicalBaseUrl = configuration["HistoricalExchangeRateAPI:BaseUrl"];
            _appId = configuration["HistoricalExchangeRateAPI:AppId"];
        }

        public async Task<Dictionary<string, decimal>> GetExchangeRatesAsync(string baseCurrency)
        {
            var url = $"{_baseUrl}{baseCurrency}?access_key={_apiKey}";

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
                throw new Exception("Network error while fetching exchange rates. Please check your connection and try again.", ex);
            }
            catch (JsonException ex)
            {
                throw new Exception("Error parsing exchange rates response. The response may be in an unexpected format.", ex);
            }
        }

        public async Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            try
            {
                var rates = await GetExchangeRatesAsync(fromCurrency);
                if (!rates.ContainsKey(toCurrency))
                {
                    throw new KeyNotFoundException($"Exchange rate for '{toCurrency}' not found.");
                }
                return rates[toCurrency];
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while retrieving the exchange rate.", ex);
            }
        }
        public async Task<Dictionary<string, string>> GetCurrenciesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_currencyUrl);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var currencies = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                return currencies;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Network error while fetching currencies. Please check your connection and try again.", ex);
            }
            catch (JsonException ex)
            {
                throw new Exception("Error parsing currencies response. The response may be in an unexpected format.", ex);
            }
        }

        public async Task<Dictionary<string, decimal>> GetHistoricalRatesAsync(DateTime date)
        {
            var url = $"{_historicalBaseUrl}{date:yyyy-MM-dd}.json?app_id={_appId}";

            try
            {
                var response = await _httpClient.GetStringAsync(url);
                var result = JsonConvert.DeserializeObject<dynamic>(response);
                var rates = result?.rates;
                if (rates == null)
                {
                    throw new Exception("Historical rates data is missing from the response.");
                }
                return rates.ToObject<Dictionary<string, decimal>>();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Network error while fetching historical rates. Please check your connection and try again.", ex);
            }
            catch (JsonException ex)
            {
                throw new Exception("Error parsing historical rates response. The response may be in an unexpected format.", ex);
            }
        }
    }
}

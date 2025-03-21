using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService(HttpClient httpClient)
        {
            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient), "HttpClient is null!");
            }

            _httpClient = httpClient;

            System.Diagnostics.Debug.WriteLine($"🌍 WeatherService HttpClient BaseAddress: {_httpClient.BaseAddress}");

            if (_httpClient.BaseAddress == null)
            {
                throw new InvalidOperationException("❌ HttpClient BaseAddress is NOT set! API calls will fail.");
            }

            _httpClient.DefaultRequestHeaders.Add("User-Agent", "HttpClient");
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<List<WeatherForecast>> GetWeatherAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("🔄 Fetching weather data from API...");

                var response = await _httpClient.GetFromJsonAsync<List<WeatherForecast>>("api/weather");

                if (response == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ No weather data received.");
                    return new List<WeatherForecast>();
                }

                System.Diagnostics.Debug.WriteLine($"✅ API call successful. Received {response.Count} items.");
                return response;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ API request error: {ex.Message}");
                return new List<WeatherForecast>();
            }
        }
    }
}

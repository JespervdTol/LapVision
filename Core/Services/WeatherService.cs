using Core.Model;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Core.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService(HttpClient httpClient)
        {
            if (httpClient == null)
                throw new ArgumentNullException(nameof(httpClient), "HttpClient is null!");

            _httpClient = httpClient;

            System.Diagnostics.Debug.WriteLine($"🌍 WeatherService HttpClient BaseAddress: {_httpClient.BaseAddress}");

            if (_httpClient.BaseAddress == null)
                throw new InvalidOperationException("❌ HttpClient BaseAddress is NOT set! API calls will fail.");

            _httpClient.DefaultRequestHeaders.Add("User-Agent", "HttpClient");
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<List<WeatherForecast>> GetWeatherAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("🔄 Fetching weather data from API...");

                var response = await _httpClient.GetAsync("api/weather");

                System.Diagnostics.Debug.WriteLine($"📡 Response Status Code: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Failed to get weather data. Status: {response.StatusCode}");
                    return new List<WeatherForecast>();
                }

                var raw = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"📝 Raw JSON Response: {raw}");

                var data = JsonSerializer.Deserialize<List<WeatherForecast>>(raw, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (data == null || data.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("❌ No weather data found or empty list returned.");
                    return new List<WeatherForecast>();
                }

                System.Diagnostics.Debug.WriteLine($"✅ Successfully received {data.Count} forecasts.");
                return data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ API request error: {ex.Message}");
                return new List<WeatherForecast>();
            }
        }
    }
}
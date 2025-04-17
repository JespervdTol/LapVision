using Core.Model;
using System.Net.Http.Json;
using System.Text.Json;

namespace API.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient), "HttpClient is null!");

            if (_httpClient.BaseAddress == null)
                throw new InvalidOperationException("❌ HttpClient BaseAddress is NOT set!");

            _httpClient.DefaultRequestHeaders.Add("User-Agent", "HttpClient");
            _httpClient.Timeout = TimeSpan.FromSeconds(30);

            System.Diagnostics.Debug.WriteLine($"🌍 WeatherService HttpClient BaseAddress: {_httpClient.BaseAddress}");
        }

        public async Task<List<WeatherForecast>> GetWeatherAsync()
        {
            System.Diagnostics.Debug.WriteLine("🔄 Fetching weather data from API...");

            try
            {
                var response = await _httpClient.GetAsync("api/weather");
                System.Diagnostics.Debug.WriteLine($"📡 Response Status Code: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Failed to get weather data. Status: {response.StatusCode}");
                    return new();
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
                    return new();
                }

                System.Diagnostics.Debug.WriteLine($"✅ Successfully received {data.Count} forecasts.");
                return data;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ API request error: {ex.Message}");
                return new();
            }
        }
    }
}
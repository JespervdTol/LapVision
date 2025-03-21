using Microsoft.AspNetCore.Mvc;
using Core.Model;
using Core.Services;
using Core.Context;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/weather")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly WeatherService _weatherService;
        private readonly DataContext _dbContext;

        public WeatherForecastController(WeatherService weatherService, DataContext dbContext)
        {
            _weatherService = weatherService;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WeatherForecast>>> Get()
        {
            System.Diagnostics.Debug.WriteLine("🚀 GET /api/weather endpoint hit");

            try
            {
                var forecasts = await _dbContext.WeatherForecasts.ToListAsync();

                if (forecasts == null || !forecasts.Any())
                {
                    System.Diagnostics.Debug.WriteLine("❌ No weather data returned.");
                    return NotFound("No weather data available.");
                }

                System.Diagnostics.Debug.WriteLine($"✅ Weather data found: {forecasts.Count} items.");
                return Ok(forecasts);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error in GetWeatherAsync: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

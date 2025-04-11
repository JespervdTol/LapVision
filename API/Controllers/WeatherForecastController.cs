using Microsoft.AspNetCore.Mvc;
using Core.Model;
using Core.Context;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/weather")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly DataContext _dbContext;

        public WeatherForecastController(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WeatherForecast>>> Get()
        {
            System.Diagnostics.Debug.WriteLine("🚀 GET /api/weather endpoint hit");

            try
            {
                var forecasts = await _dbContext.WeatherForecasts.ToListAsync();

                if (forecasts == null || forecasts.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("❌ No weather data found.");
                    return NotFound("No weather data available.");
                }

                System.Diagnostics.Debug.WriteLine($"✅ Weather data returned: {forecasts.Count} items.");
                return Ok(forecasts);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error in GET /api/weather: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
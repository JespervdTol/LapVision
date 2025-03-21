using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
    public static class ApiConnectionHelper
    {
        public static async Task<bool> IsApiAccessibleAsync(HttpClient client)
        {
            try
            {
                var response = await client.GetAsync("weatherforecast");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ API connection failed: {ex.Message}");
                return false;
            }
        }
    }
}

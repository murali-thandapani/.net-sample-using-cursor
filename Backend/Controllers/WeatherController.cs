using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.API.Services;

namespace UserManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WeatherController : ControllerBase
{
    private readonly WeatherService _weatherService;

    public WeatherController(WeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [HttpGet("{city}")]
    public async Task<IActionResult> GetWeather(string city)
    {
        var weather = await _weatherService.GetWeatherAsync(city);
        
        if (weather == null)
        {
            return NotFound(new { message = $"Weather data not found for {city}" });
        }

        return Ok(weather);
    }

    [HttpGet("cities")]
    public IActionResult GetIndianCities()
    {
        var cities = new[]
        {
            "Mumbai", "Delhi", "Bangalore", "Hyderabad", "Chennai",
            "Kolkata", "Pune", "Ahmedabad", "Jaipur", "Surat"
        };

        return Ok(cities);
    }
}

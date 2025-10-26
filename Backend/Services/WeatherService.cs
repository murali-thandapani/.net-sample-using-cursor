using StackExchange.Redis;
using System.Text.Json;

namespace UserManagement.API.Services;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly IDatabase _redis;
    private readonly ILogger<WeatherService> _logger;

    public WeatherService(IHttpClientFactory httpClientFactory, IConnectionMultiplexer redis, ILogger<WeatherService> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");
        _redis = redis.GetDatabase();
        _logger = logger;
    }

    public async Task<WeatherInfo?> GetWeatherAsync(string city)
    {
        try
        {
            // Check cache first
            try
            {
                var cacheKey = $"weather:{city.ToLower()}";
                var cachedWeather = await _redis.StringGetAsync(cacheKey);
                
                if (cachedWeather.HasValue)
                {
                    _logger.LogInformation($"Cache hit for {city}");
                    return JsonSerializer.Deserialize<WeatherInfo>(cachedWeather!);
                }
            }
            catch (Exception cacheEx)
            {
                _logger.LogWarning($"Redis cache unavailable: {cacheEx.Message}");
            }

            // Generate mock weather data (for demo purposes)
            var random = new Random();
            var weatherInfo = new WeatherInfo
            {
                City = city,
                Country = "IN",
                Temperature = Math.Round(20 + random.NextDouble() * 15, 1),
                FeelsLike = Math.Round(18 + random.NextDouble() * 15, 1),
                Humidity = random.Next(40, 90),
                Pressure = random.Next(1000, 1020),
                Description = new[] { "Sunny", "Cloudy", "Partly Cloudy", "Clear", "Hazy" }[random.Next(5)],
                WindSpeed = Math.Round(random.NextDouble() * 20, 1),
                Visibility = Math.Round(5 + random.NextDouble() * 5, 1)
            };

            // Cache for 10 minutes
            try
            {
                var cacheKey = $"weather:{city.ToLower()}";
                var expiry = TimeSpan.FromMinutes(10);
                await _redis.StringSetAsync(cacheKey, JsonSerializer.Serialize(weatherInfo), expiry);
                _logger.LogInformation($"Weather data cached for {city}");
            }
            catch (Exception cacheEx)
            {
                _logger.LogWarning($"Failed to cache weather data: {cacheEx.Message}");
            }
            
            return weatherInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching weather for {city}");
            return null;
        }
    }
}

public class WeatherInfo
{
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public double FeelsLike { get; set; }
    public int Humidity { get; set; }
    public int Pressure { get; set; }
    public string Description { get; set; } = string.Empty;
    public double WindSpeed { get; set; }
    public double Visibility { get; set; }
}

public class WeatherApiResponse
{
    public string Name { get; set; } = string.Empty;
    public Main Main { get; set; } = new();
    public Wind Wind { get; set; } = new();
    public Sys Sys { get; set; } = new();
    public List<Weather> Weather { get; set; } = new();
    public int Visibility { get; set; }
}

public class Main
{
    public double Temp { get; set; }
    public double FeelsLike { get; set; }
    public int Humidity { get; set; }
    public int Pressure { get; set; }
}

public class Wind
{
    public double Speed { get; set; }
}

public class Sys
{
    public string Country { get; set; } = string.Empty;
}

public class Weather
{
    public string Description { get; set; } = string.Empty;
}

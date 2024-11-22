using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;


namespace SpeechRecognition
{
    public class Weather
    {
        private readonly string _apiKey;

        public Weather(string apiKey)
        {
            _apiKey = "bd5e378503939ddaee76f12ad7a97608";
        }

        public async Task<string> GetWeatherAsync(string city)
        {
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=imperial";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var weatherData = JsonDocument.Parse(json);
                    var temperature = weatherData.RootElement.GetProperty("main").GetProperty("temp").GetDouble();
                    var description = weatherData.RootElement.GetProperty("weather")[0].GetProperty("description").GetString();
                    return $"The current temperature in {city} is {temperature}°F with {description}.";
                }
                else
                {
                    return "Unable to fetch weather data. Please try again.";
                }
            }
        }
    }
}
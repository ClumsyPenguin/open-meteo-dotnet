using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Globalization;

namespace OpenMeteo
{
    /// <summary>
    /// Handles GET Requests and performs API Calls.
    /// </summary>
    public class OpenMeteoClient
    {
        private const string WeatherApiUrl = "https://api.open-meteo.com/v1/forecast";
        private const string GeocodeApiUrl = "https://geocoding-api.open-meteo.com/v1/search";
        private const string AirQualityApiUrl = "https://air-quality-api.open-meteo.com/v1/air-quality";
        private readonly HttpController _httpController;

        /// <summary>
        /// Creates a new <seealso cref="OpenMeteoClient"/> object and sets the neccessary variables (httpController, CultureInfo)
        /// </summary>
        public OpenMeteoClient()
        {
            _httpController = new HttpController();
        }

        /// <summary>
        /// Performs two GET-Requests (first geocoding api for latitude,longitude, then weather forecast)
        /// </summary>
        /// <param name="location">Name of city</param>
        /// <returns>If successful returns an awaitable Task containing WeatherForecast or NULL if request failed</returns>
        public async Task<WeatherForecast?> QueryAsync(string location)
        {
            var geocodingOptions = new GeocodingOptions(location);

            // Get location Information
            var response = await GetGeocodingDataAsync(geocodingOptions);
            if (response?.Locations == null)
                return null;

            var options = new WeatherForecastOptions
            {
                Latitude = response.Locations[0].Latitude,
                Longitude = response.Locations[0].Longitude,
                Current = CurrentOptions.All // Get all current weather data if nothing else is provided
                
            };

            return await GetWeatherForecastAsync(options);
        }

        /// <summary>
        /// Performs two GET-Requests (first geocoding api for latitude,longitude, then weather forecast)
        /// </summary>
        /// <param name="options">Geocoding options</param>
        /// <returns>If successful awaitable <see cref="Task"/> or NULL</returns>
        public async Task<WeatherForecast?> QueryAsync(GeocodingOptions options)
        {
            // Get City Information
            var response = await GetLocationDataAsync(options);
            if (response?.Locations == null)
                return null;

            var weatherForecastOptions = new WeatherForecastOptions
            {
                Latitude = response.Locations[0].Latitude,
                Longitude = response.Locations[0].Longitude,
                Current = CurrentOptions.All // Get all current weather data if nothing else is provided
                
            };

            return await GetWeatherForecastAsync(weatherForecastOptions);
        }

        /// <summary>
        /// Performs one GET-Request
        /// </summary>
        /// <param name="options"></param>
        /// <returns>Awaitable Task containing WeatherForecast or NULL</returns>
        public async Task<WeatherForecast?> QueryAsync(WeatherForecastOptions options)
        {
            try
            {
                return await GetWeatherForecastAsync(options);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Performs one GET-Request to get weather information
        /// </summary>
        /// <param name="latitude">City latitude</param>
        /// <param name="longitude">City longitude</param>
        /// <returns>Awaitable Task containing WeatherForecast or NULL</returns>
        public async Task<WeatherForecast?> QueryAsync(float latitude, float longitude)
        {
            var options = new WeatherForecastOptions
            {
                Latitude = latitude,
                Longitude = longitude,
                
            };
            return await QueryAsync(options);
        }

        /// <summary>
        /// Gets Weather Forecast for a given location with individual options
        /// </summary>
        /// <param name="location"></param>
        /// <param name="options"></param>
        /// <returns><see cref="WeatherForecast"/> for the FIRST found result for <paramref name="location"/></returns>
        public async Task<WeatherForecast?> QueryAsync(string location, WeatherForecastOptions options)
        {
            var geocodingApiResponse = await GetLocationDataAsync(location);
            if (geocodingApiResponse?.Locations == null)
                return null;
            
            options.Longitude = geocodingApiResponse.Locations[0].Longitude;
            options.Latitude = geocodingApiResponse.Locations[0].Latitude;

            return await GetWeatherForecastAsync(options);
        }

        /// <summary>
        /// Gets air quality data for a given location with individual options
        /// </summary>
        /// <param name="options">options for air quality request</param>
        /// <returns><see cref="AirQuality"/> if successfull or <see cref="null"/> if failed</returns>
        public async Task<AirQuality?> QueryAsync(AirQualityOptions options)
        {
            return await GetAirQualityAsync(options);
        }

        /// <summary>
        /// Performs one GET-Request to Open-Meteo Geocoding API 
        /// </summary>
        /// <param name="location">Name of a location or city</param>
        /// <returns></returns>
        public async Task<GeocodingApiResponse?> GetLocationDataAsync(string location)
        {
            var geocodingOptions = new GeocodingOptions(location);

            return await GetLocationDataAsync(geocodingOptions);
        }

        public async Task<GeocodingApiResponse?> GetLocationDataAsync(GeocodingOptions options)
        {
            return await GetGeocodingDataAsync(options);
        }

        /// <summary>
        /// Performs one GET-Request to get a (float, float) tuple
        /// </summary>
        /// <param name="location">Name of a city or location</param>
        /// <returns>(latitude, longitude) tuple of first found location or null if no location was found</returns>
        public async Task<(float latitude, float longitude)?> GetLocationLatitudeLongitudeAsync(string location)
        {
            var response = await GetLocationDataAsync(location);
            if (response?.Locations == null)
                return null;
            return (response.Locations[0].Latitude, response.Locations[0].Longitude);
        }

        public WeatherForecast? Query(WeatherForecastOptions options)
        {
            return QueryAsync(options).GetAwaiter().GetResult();
        }
        
        public AirQuality? Query(AirQualityOptions options)
        {
            return QueryAsync(options).GetAwaiter().GetResult();
        }

        private async Task<AirQuality?> GetAirQualityAsync(AirQualityOptions options)
        {
            try
            {
                var response = await _httpController.Client.GetAsync(MergeUrlWithOptions(AirQualityApiUrl, options));
                response.EnsureSuccessStatusCode();

                var airQuality = await JsonSerializer.DeserializeAsync<AirQuality>(await response.Content.ReadAsStreamAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                return airQuality;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Converts a given weathercode to it's string representation
        /// </summary>
        /// <param name="weathercode"></param>
        /// <returns><see cref="string"/> Weathercode string representation</returns>
        public string WeathercodeToString(int weathercode)
        {
            return weathercode switch
            {
                0 => "Clear sky",
                1 => "Mainly clear",
                2 => "Partly cloudy",
                3 => "Overcast",
                45 => "Fog",
                48 => "Depositing rime Fog",
                51 => "Light drizzle",
                53 => "Moderate drizzle",
                55 => "Dense drizzle",
                56 => "Light freezing drizzle",
                57 => "Dense freezing drizzle",
                61 => "Slight rain",
                63 => "Moderate rain",
                65 => "Heavy rain",
                66 => "Light freezing rain",
                67 => "Heavy freezing rain",
                71 => "Slight snow fall",
                73 => "Moderate snow fall",
                75 => "Heavy snow fall",
                77 => "Snow grains",
                80 => "Slight rain showers",
                81 => "Moderate rain showers",
                82 => "Violent rain showers",
                85 => "Slight snow showers",
                86 => "Heavy snow showers",
                95 => "Thunderstorm",
                96 => "Thunderstorm with light hail",
                99 => "Thunderstorm with heavy hail",
                _ => "Invalid weathercode"
            };
        }

        private async Task<WeatherForecast?> GetWeatherForecastAsync(WeatherForecastOptions options)
        {
            try
            {
                var response = await _httpController.Client.GetAsync(MergeUrlWithOptions(WeatherApiUrl, options));
                response.EnsureSuccessStatusCode();

                var weatherForecast = await JsonSerializer.DeserializeAsync<WeatherForecast>(await response.Content.ReadAsStreamAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                return weatherForecast;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return null;
            }

        }

        private async Task<GeocodingApiResponse?> GetGeocodingDataAsync(GeocodingOptions options)
        {
            try
            {
                var response = await _httpController.Client.GetAsync(MergeUrlWithOptions(GeocodeApiUrl, options));
                response.EnsureSuccessStatusCode();

                var geocodingData = await JsonSerializer.DeserializeAsync<GeocodingApiResponse>(await response.Content.ReadAsStreamAsync(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                return geocodingData;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Can't find " + options.Name + ". Please make sure that the name is valid.");
                Console.WriteLine(e.Message);
                return null;
            }
        }

        private string MergeUrlWithOptions(string url, WeatherForecastOptions? options)
        {
            if (options == null) return url;

            var uri = new UriBuilder(url);
            var isFirstParam = false;

            // If no query given, add '?' to start the query string
            if (uri.Query == string.Empty)
            {
                uri.Query = "?";

                // isFirstParam becomes true because the query string is new
                isFirstParam = true;
            }

            // Add the properties
            
            // Begin with Latitude and Longitude since they're required
            if (isFirstParam)
                uri.Query += "latitude=" +  options.Latitude.ToString(CultureInfo.InvariantCulture);
            else
                uri.Query += "&latitude=" + options.Latitude.ToString(CultureInfo.InvariantCulture);

            uri.Query += "&longitude=" + options.Longitude.ToString(CultureInfo.InvariantCulture);

            uri.Query += "&temperature_unit=" + options.Temperature_Unit;
            uri.Query += "&windspeed_unit=" + options.Windspeed_Unit;
            uri.Query += "&precipitation_unit=" + options.Precipitation_Unit;
            if (options.Timezone != string.Empty)
                uri.Query += "&timezone=" + options.Timezone;

            uri.Query += "&timeformat=" + options.Timeformat;

            uri.Query += "&past_days=" + options.Past_Days;

            if (options.Start_date != string.Empty)
                uri.Query += "&start_date=" + options.Start_date;
            if (options.End_date != string.Empty)
                uri.Query += "&end_date=" + options.End_date;

            // Now we iterate through hourly and daily

            // Hourly
            if (options.Hourly.Count > 0)
            {
                var firstHourlyElement = true;
                uri.Query += "&hourly=";

                foreach (var option in options.Hourly)
                {
                    if (firstHourlyElement)
                    {
                        uri.Query += option.ToString();
                        firstHourlyElement = false;
                    }
                    else
                    {
                        uri.Query += "," + option;
                    }
                }
            }

            // Daily
            if (options.Daily.Count > 0)
            {
                var firstDailyElement = true;
                uri.Query += "&daily=";
                foreach (var option in options.Daily)
                {
                    if (firstDailyElement)
                    {
                        uri.Query += option.ToString();
                        firstDailyElement = false;
                    }
                    else
                    {
                        uri.Query += "," + option;
                    }
                }
            }

            // 0.2.0 Weather models
            // cell_selection
            uri.Query += "&cell_selection=" + options.Cell_Selection;

            // Models
            if (options.Models.Count > 0)
            {
                var firstModelsElement = true;
                uri.Query += "&models=";
                foreach (var option in options.Models)
                {
                    if (firstModelsElement)
                    {
                        uri.Query += option.ToString();
                        firstModelsElement = false;
                    }
                    else
                    {
                        uri.Query += "," + option;
                    }
                }
            }

            // new current parameter
            if (options.Current.Count > 0)
            {
                var firstCurrentElement = true;
                uri.Query += "&current=";
                foreach (var option in options.Current)
                {
                    if (firstCurrentElement)
                    {
                        uri.Query += option.ToString();
                        firstCurrentElement = false;
                    }
                    else
                    {
                        uri.Query += "," + option;
                    }
                }
            }

            // new minutely_15 parameter
            if (options.Minutely15.Count > 0)
            {
                var firstMinutelyElement = true;
                uri.Query += "&minutely_15=";
                foreach (var option in options.Minutely15)
                {
                    if (firstMinutelyElement)
                    {
                        uri.Query += option.ToString();
                        firstMinutelyElement = false;
                    }
                    else
                    {
                        uri.Query += "," + option;
                    }
                }
            }

            return uri.ToString();
        }

        /// <summary>
        /// Combines a given url with an options object to create a url for GET requests
        /// </summary>
        /// <returns>url+queryString</returns>
        private string MergeUrlWithOptions(string url, GeocodingOptions options)
        {
            if (options == null) return url;

            var uri = new UriBuilder(url);
            var isFirstParam = false;

            // If no query given, add '?' to start the query string
            if (uri.Query == string.Empty)
            {
                uri.Query = "?";

                // isFirstParam becomes true because the query string is new
                isFirstParam = true;
            }

            // Now we check every property and set the value, if neccessary
            if (isFirstParam)
                uri.Query += "name=" + options.Name;
            else
                uri.Query += "&name=" + options.Name;

            if(options.Count >0)
                uri.Query += "&count=" + options.Count;
            
            if (options.Format != string.Empty)
                uri.Query += "&format=" + options.Format;

            if (options.Language != string.Empty)
                uri.Query += "&language=" + options.Language;

            return uri.ToString();
        }

        /// <summary>
        /// Combines a given url with an options object to create a url for GET requests
        /// </summary>
        /// <returns>url+queryString</returns>
        private string MergeUrlWithOptions(string url, AirQualityOptions options)
        {
            if (options is null) 
                return url;

            var uri = new UriBuilder(url);
            var isFirstParam = false;

            // If no query given, add '?' to start the query string
            if (uri.Query == string.Empty)
            {
                uri.Query = "?";

                // isFirstParam becomes true because the query string is new
                isFirstParam = true;
            }

            // Now we check every property and set the value, if neccessary
            if (isFirstParam)
                uri.Query += "latitude=" + options.Latitude.ToString(CultureInfo.InvariantCulture);
            else
                uri.Query += "&latitude=" + options.Latitude.ToString(CultureInfo.InvariantCulture);

            uri.Query += "&longitude=" + options.Longitude.ToString(CultureInfo.InvariantCulture);

            if (options.Domains != string.Empty)
                uri.Query += "&domains=" + options.Domains;

            if (options.Timeformat != string.Empty)
                uri.Query += "&timeformat=" + options.Timeformat;

            if (options.Timezone != string.Empty)
                uri.Query += "&timezone=" + options.Timezone;

          
            if (options.Hourly.Count < 0) 
                return uri.ToString();
            
            // Finally add hourly array
            var firstHourlyElement = true;
            uri.Query += "&hourly=";
            foreach (var option in options.Hourly)
            {
                if (firstHourlyElement)
                {
                    uri.Query += option.ToString();
                    firstHourlyElement = false;
                }
                else
                {
                    uri.Query += "," + option;
                }
            }

            return uri.ToString();
        }
    }
}


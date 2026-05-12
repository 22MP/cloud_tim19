using SmartApiary.Simulator.NsDevice;
using SmartApiary.Simulator.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace SmartApiary.Simulator.Simulator
{
    public class Simulator
    {
        private readonly string TELEMETRY_ENDPOINT = "http://localhost:5062/api/devices/telemetry";
        private readonly Device _device;

        public Simulator(Device device)
        {
            _device = device;
        }

        public async Task Start()
        {
            Console.WriteLine("");
            while (true)
            {
                if(_device.Battery <= 0)
                {
                    Console.WriteLine("Battery is 0%, shuting device down...");
                    break;
                }

                Measurement newMeasurement = _device.GenerateSample();

                var measurement = new
                {
                    WeightKg = newMeasurement.WeightKg,
                    Humidity = newMeasurement.Humidity,
                    Temperature = newMeasurement.Temperature,
                    BatteryPercentage = _device.Battery,
                    RecordedAt = DateTime.UtcNow
                };

                await SendTelemetry(_device.AccessToken!, measurement);

                await Task.Delay(5000);
            }
        }

        async Task SendTelemetry(string token, object measurement)
        {
            using var http = new HttpClient();
            http.DefaultRequestHeaders.Add("X-Device-Token", token);

            var content = new StringContent(JsonSerializer.Serialize(measurement), Encoding.UTF8, "application/json");

            try
            {
                var response = await http.PostAsync(TELEMETRY_ENDPOINT, content);

                if (response.IsSuccessStatusCode)
                {
                    var m = (dynamic)measurement;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Sent: " + $"Weight={((JsonElement)JsonSerializer.SerializeToElement(measurement)).GetProperty("WeightKg")}kg");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode}: {error}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while sending: {ex.Message}");
            }
        }
    }
}
